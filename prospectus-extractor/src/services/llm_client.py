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
            ),
            mode=instructor.Mode.JSON,
        )
        self.model_name = settings.llm_model_name

    async def _extract_section(
        self, 
        chunks: List[TextChunk], 
        response_model: Type[T], 
        section_tags: List[str], 
        prompt_instruction: str
    ) -> T:
        relevant_chunks = [c for c in chunks if c.section_label and c.section_label.lower() in section_tags]
        if not relevant_chunks:
            relevant_chunks = chunks[:15]

        context_text = "\n\n".join([c.text for c in relevant_chunks])

        try:
            return await self.client.chat.completions.create(
                model=self.model_name,
                response_model=response_model,
                messages=[
                    {
                        "role": "system", 
                        "content": "You are a precise data extraction assistant. Extract data strictly based on the provided text."
                    },
                    {
                        "role": "user", 
                        "content": f"{prompt_instruction}\n\nDATA:\n{context_text}"
                    }
                ],
                max_retries=2,
            )
        except Exception as e:
            logger.error(f"Failed to extract {response_model.__name__}: {e}")
            return response_model()

    async def extract_university_info(self, chunks: List[TextChunk]) -> UniversityInfo:
        """Extract university name, short name, and location."""
        first_chunks = chunks[:5]
        context_text = "\n\n".join([c.text for c in first_chunks])

        try:
            return await self.client.chat.completions.create(
                model=self.model_name,
                response_model=UniversityInfo,
                messages=[
                    {
                        "role": "system",
                        "content": "Extract the university's basic information from the prospectus."
                    },
                    {
                        "role": "user",
                        "content": f"Extract the university name, short name (if any), and location.\n\nDATA:\n{context_text}"
                    }
                ],
                max_retries=2,
            )
        except Exception as e:
            logger.error(f"Failed to extract university info: {e}")
            return UniversityInfo()

    async def extract_all(self, chunks: List[TextChunk]) -> UniversityExtraction:
        logger.info(f"Starting parallel extraction on {len(chunks)} chunks")

        uni_task = self.extract_university_info(chunks)
        
        dept_task = self._extract_section(
            chunks, DepartmentList, ["departments", "programs", "general"], 
            "Extract all academic departments and their programs."
        )
        
        fac_task = self._extract_section(
            chunks, FacilityList, ["facilities"], 
            "Extract all campus facilities."
        )
        
        fee_task = self._extract_section(
            chunks, FeeList, ["fees"], 
            "Extract all tuition and fee information."
        )
        
        adm_task = self._extract_section(
            chunks, AdmissionInfo, ["admissions"], 
            "Extract admission criteria and deadlines."
        )

        results = await asyncio.gather(
            uni_task, dept_task, fac_task, fee_task, adm_task, 
            return_exceptions=True
        )

        uni_info = results[0] if not isinstance(results[0], Exception) else UniversityInfo()
        dept_data = results[1] if not isinstance(results[1], Exception) else DepartmentList()
        fac_data = results[2] if not isinstance(results[2], Exception) else FacilityList()
        fee_data = results[3] if not isinstance(results[3], Exception) else FeeList()
        admissions = results[4] if not isinstance(results[4], Exception) else None

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