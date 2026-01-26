import logging
import instructor
import asyncio
from openai import AsyncOpenAI
from typing import List, Optional, Type, TypeVar
from pydantic import BaseModel
from datetime import datetime

from ..config import settings
from ..models.schema import (
    UniversityExtraction, Department, Facility, FeeItem, 
    AdmissionInfo, ExtractionMetaData
)
from .chunker import TextChunk

logger = logging.getLogger(__name__)

T = TypeVar("T", bound=BaseModel)


class DepartmentList(BaseModel):
    items: List[Department] = []

class FacilityList(BaseModel):
    items: List[Facility] = []

class FeeList(BaseModel):
    items: List[FeeItem] = []

class UniversityInfo(BaseModel):
    name: str = "Unknown University"
    short_name: Optional[str] = None
    location: Optional[str] = None


class ExtractionService:
    def __init__(self):
        self.client = instructor.from_openai(
            AsyncOpenAI(
                base_url=settings.llm_base_url,
                api_key="ollama",
                timeout=600,  # 10 minutes for large models
            ),
            mode=instructor.Mode.JSON,
        )
        self.model_name = settings.llm_model_name
        self.semaphore = asyncio.Semaphore(5)  # Allow 5 concurrent requests on 8 vCPU VM (7b model)

    async def _extract_batch(
        self, 
        chunks: List[TextChunk], 
        response_model: Type[T], 
        prompt_instruction: str
    ) -> T:
        context_text = "\n\n".join([c.text for c in chunks])
        max_context_chars = 20000
        if len(context_text) > max_context_chars:
            logger.warning(f"Truncating context from {len(context_text)} to {max_context_chars} chars")
            context_text = context_text[:max_context_chars] + "\n\n[TRUNCATED]"
        
        try:
            async with self.semaphore:
                result = await self.client.chat.completions.create(
                    model=self.model_name,
                    response_model=response_model,
                    messages=[
                        {
                            "role": "system", 
                            "content": "You are a precise data extraction assistant. Extract data strictly based on the provided text. Return valid JSON."
                        },
                        {
                            "role": "user", 
                            "content": f"{prompt_instruction}\n\nDATA:\n{context_text}"
                        }
                    ],
                    max_retries=3,
                )
                return result
        except Exception as e:
            logger.error(f"Failed to extract batch for {response_model.__name__}: {type(e).__name__}: {e}")
            return response_model()

    async def _extract_section(
        self, 
        chunks: List[TextChunk], 
        response_model: Type[T], 
        section_tags: List[str], 
        prompt_instruction: str,
        batch_size: int = 15  # Larger batches, fewer requests
    ) -> T:
        relevant_chunks = [c for c in chunks if c.section_label and c.section_label.lower() in section_tags]
        if not relevant_chunks:
             relevant_chunks = chunks[:5]
        
        chunk_batches = [relevant_chunks[i:i + batch_size] for i in range(0, len(relevant_chunks), batch_size)]
        logger.info(f"Processing {len(relevant_chunks)} chunks for {response_model.__name__} in {len(chunk_batches)} batches (sequential).")
        results = []
        for i, batch in enumerate(chunk_batches):
            logger.info(f"  Batch {i+1}/{len(chunk_batches)} for {response_model.__name__}...")
            result = await self._extract_batch(batch, response_model, prompt_instruction)
            results.append(result)
        
        # Aggregate results
        final_result = response_model()
        list_field = None
        for name, field in response_model.model_fields.items():
            if "List" in str(field.annotation) or "list" in str(field.annotation):
                list_field = name
                break
                
        for res in results:
            if isinstance(res, Exception):
                logger.error(f"Batch failed: {res}")
                continue
            
            # Merge lists
            if list_field and hasattr(res, list_field):
                current_list = getattr(final_result, list_field)
                batch_list = getattr(res, list_field)
                if batch_list:
                    current_list.extend(batch_list)
            if response_model == AdmissionInfo:
                if res.eligibility_criteria:
                    final_result.eligibility_criteria = (final_result.eligibility_criteria or "") + "\n" + res.eligibility_criteria
                if res.important_dates:
                    final_result.important_dates.extend(res.important_dates)

        return final_result

    async def extract_university_info(self, chunks: List[TextChunk]) -> UniversityInfo:
        """Extract university name, short name, and location."""
        first_chunks = chunks[:5]
        context_text = "\n\n".join([c.text for c in first_chunks])
        # Truncate if needed
        if len(context_text) > 4000:
            context_text = context_text[:4000]

        try:
            async with self.semaphore:
                return await self.client.chat.completions.create(
                    model=self.model_name,
                    response_model=UniversityInfo,
                    messages=[
                        {
                            "role": "system",
                            "content": "Extract the university's basic information from the prospectus. Return valid JSON."
                        },
                        {
                            "role": "user",
                            "content": f"Extract the university name, short name (if any), and location.\n\nDATA:\n{context_text}"
                        }
                    ],
                    max_retries=3,
                )
        except Exception as e:
            logger.error(f"Failed to extract university info: {type(e).__name__}: {e}")
            return UniversityInfo()

    async def extract_all(self, chunks: List[TextChunk]) -> UniversityExtraction:
        logger.info(f"Starting sequential extraction on {len(chunks)} chunks (optimized for VM Ollama)")
        logger.info("Step 1/5: Extracting university info...")
        try:
            uni_info = await self.extract_university_info(chunks)
        except Exception as e:
            logger.error(f"University info extraction failed: {e}")
            uni_info = UniversityInfo()

        logger.info("Step 2/5: Extracting departments...")
        try:
            dept_data = await self._extract_section(
                chunks, DepartmentList, ["departments", "programs", "general"], 
                "Extract all academic departments and their programs. For each department, list its name and programs offered."
            )
        except Exception as e:
            logger.error(f"Department extraction failed: {e}")
            dept_data = DepartmentList()
        
        logger.info("Step 3/5: Extracting facilities...")
        try:
            fac_data = await self._extract_section(
                chunks, FacilityList, ["facilities"], 
                "Extract all campus facilities."
            )
        except Exception as e:
            logger.error(f"Facility extraction failed: {e}")
            fac_data = FacilityList()
        
        logger.info("Step 4/5: Extracting fees...")
        try:
            fee_data = await self._extract_section(
                chunks, FeeList, ["fees"], 
                "Extract all tuition and fee information."
            )
        except Exception as e:
            logger.error(f"Fee extraction failed: {e}")
            fee_data = FeeList()
        
        logger.info("Step 5/5: Extracting admissions...")
        try:
            admissions = await self._extract_section(
                chunks, AdmissionInfo, ["admissions"], 
                "Extract admission criteria and deadlines."
            )
        except Exception as e:
            logger.error(f"Admission extraction failed: {e}")
            admissions = None

        logger.info(f"Extracted university: {uni_info.name}")
        logger.info(f"Extracted {len(dept_data.items)} departments")
        logger.info(f"Extracted {len(fac_data.items)} facilities")
        logger.info(f"Extracted {len(fee_data.items)} fees")

        metadata = ExtractionMetaData(
            extraction_timestamp=datetime.now(),
            total_chunks_processed=len(chunks),
            total_pages=max(c.page_number for c in chunks) if chunks else 0,
            confidence_scores={
                "departments": 0.8 if dept_data.items else 0.0,
                "facilities": 0.8 if fac_data.items else 0.0,
                "fees": 0.8 if fee_data.items else 0.0,
                "admissions": 0.8 if admissions else 0.0,
            }
        )

        return UniversityExtraction(
            university_name=uni_info.name,
            university_short_name=uni_info.short_name,
            location=uni_info.location,
            departments=dept_data.items,
            facilities=fac_data.items,
            fee_structure=fee_data.items,
            admissions=admissions,
            contact=None,
            metadata=metadata
        )

extraction_service = ExtractionService()