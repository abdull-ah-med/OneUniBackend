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
    AdmissionInfo, ExtractionMetaData, Program
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


# Generic extraction prompts - designed to work with any university prospectus
PROMPTS = {
    "university_info": """Extract the university's basic information from this prospectus.

Look for:
- Full university name
- Short name or abbreviation (if mentioned)
- Main campus location/city

Return the information as JSON.""",

    "departments": """Extract ALL academic departments and their degree programs from this text.

IMPORTANT INSTRUCTIONS:
- Look for patterns like "Department of [Name]", "Faculty of [Name]", "School of [Name]"
- For each department, extract ALL programs/degrees listed under it
- Programs typically start with: B.Sc., BS, BA, B.Tech, M.Sc., MS, MA, MBA, M.Tech, PhD, Doctor
- Include program duration (e.g., 4 years, 2 years) if mentioned
- Look for credit hours in curriculum tables

EXTRACT EVERY DEPARTMENT YOU FIND. Do not summarize or skip any.

Return a complete list of all departments with their programs.""",

    "facilities": """Extract ALL campus facilities mentioned in this text.

Look for:
- Laboratories (research labs, computer labs, science labs)
- Libraries
- Hostels/dormitories
- Sports facilities (gyms, fields, courts)
- Research centers
- Cafeterias/food courts
- Auditoriums/conference halls
- Medical facilities/clinics

For each facility, note:
- Name
- Type (laboratory, library, hostel, sports, etc.)
- Description (if available)
- Capacity or features (if mentioned)

Return ALL facilities found.""",

    "fees": """Extract ALL fee and tuition information from this text.

Look for:
- Tuition fees (per semester, per year, or total)
- Admission/registration fees
- Lab fees
- Hostel/accommodation fees
- Examination fees
- Any other charges or dues

For each fee item, extract:
- Fee type (tuition, admission, lab, hostel, etc.)
- Amount (numerical value)
- Currency (use the local currency mentioned)
- Which program it applies to (if specific to certain programs)
- Payment frequency (per semester, per year, one-time)

Return ALL fee information found.""",

    "admissions": """Extract ALL admission-related information from this text.

Look for:

1. ELIGIBILITY CRITERIA:
   - Minimum marks/grades/GPA required
   - Required subjects or educational background
   - Age requirements (if any)
   - Entry test or standardized test requirements

2. APPLICATION PROCESS:
   - How to apply (online portal, paper forms)
   - Required documents
   - Application fee

3. IMPORTANT DATES:
   - Application deadlines
   - Test dates
   - Result announcement dates
   - Semester/term start dates

4. SEAT ALLOCATION:
   - Total seats available
   - Category-wise distribution (if mentioned)

Return all admission information found.""",
}


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
        self.semaphore = asyncio.Semaphore(5)

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
                            "content": "You are a precise data extraction assistant specialized in university prospectuses. Extract ALL relevant data strictly based on the provided text. Return valid JSON with complete information. Do not skip or summarize - extract everything you find."
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

    def _get_relevant_chunks(
        self, 
        chunks: List[TextChunk], 
        primary_tags: List[str],
        fallback_tags: List[str] = None
    ) -> List[TextChunk]:
        """
        Get relevant chunks with flexible filtering.
        Uses primary tags first, falls back to additional tags, then uses all chunks.
        """
        relevant = [
            c for c in chunks 
            if c.section_label and c.section_label.lower() in primary_tags
        ]
        
        if relevant:
            logger.info(f"Found {len(relevant)} chunks matching primary tags: {primary_tags}")
            return relevant
        
        # Second try: fallback tags
        if fallback_tags:
            relevant = [
                c for c in chunks 
                if c.section_label and c.section_label.lower() in fallback_tags
            ]
            if relevant:
                logger.info(f"Found {len(relevant)} chunks matching fallback tags: {fallback_tags}")
                return relevant
        
        # Third try: keyword search in chunk text (generic keywords)
        keyword_map = {
            "departments": ["department", "faculty", "school of", "college of"],
            "programs": ["bachelor", "master", "degree", "program", "course"],
            "curriculum": ["credit", "semester", "course", "syllabus"],
            "fees": ["fee", "tuition", "payment", "cost", "dues"],
            "admissions": ["admission", "eligibility", "apply", "entrance", "requirement"],
            "facilities": ["laboratory", "lab", "library", "hostel", "facility"],
        }
        
        keywords = []
        for tag in primary_tags:
            if tag in keyword_map:
                keywords.extend(keyword_map[tag])
        
        if keywords:
            relevant = [
                c for c in chunks
                if any(kw in c.text.lower() for kw in keywords)
            ]
            if relevant:
                logger.info(f"Found {len(relevant)} chunks via keyword search")
                return relevant
        
        # Final fallback: return all chunks
        logger.warning(f"No section matches for {primary_tags}, using all chunks")
        return chunks

    def _sample_chunks_evenly(self, chunks: List[TextChunk], max_count: int) -> List[TextChunk]:
        """
        Sample chunks evenly from the list to maintain coverage across the document.
        This is better than just taking the first N chunks.
        """
        if len(chunks) <= max_count:
            return chunks
        
        # Sample evenly to maintain document coverage
        step = len(chunks) / max_count
        sampled = []
        for i in range(max_count):
            index = int(i * step)
            sampled.append(chunks[index])
        
        logger.info(f"Sampled {len(sampled)} chunks evenly from {len(chunks)} (step={step:.2f})")
        return sampled

    async def _extract_section(
        self, 
        chunks: List[TextChunk], 
        response_model: Type[T], 
        primary_tags: List[str],
        prompt_instruction: str,
        fallback_tags: List[str] = None,
        batch_size: int = 15,
        max_chunks: int = 120  # Increased for better coverage
    ) -> T:
        """Extract data for a section with flexible chunk selection."""
        relevant_chunks = self._get_relevant_chunks(chunks, primary_tags, fallback_tags)
        
        # Sample chunks evenly if we have too many (better than truncating)
        if len(relevant_chunks) > max_chunks:
            relevant_chunks = self._sample_chunks_evenly(relevant_chunks, max_chunks)
        
        chunk_batches = [relevant_chunks[i:i + batch_size] for i in range(0, len(relevant_chunks), batch_size)]
        logger.info(f"Processing {len(relevant_chunks)} chunks for {response_model.__name__} in {len(chunk_batches)} batches")
        
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
            
            # Special handling for AdmissionInfo (string fields need concatenation)
            if response_model == AdmissionInfo and res:
                if res.eligibility_criteria:
                    final_result.eligibility_criteria = (final_result.eligibility_criteria or "") + "\n" + res.eligibility_criteria
                if res.application_process:
                    final_result.application_process = (final_result.application_process or "") + "\n" + res.application_process
                if res.important_dates:
                    if final_result.important_dates is None:
                        final_result.important_dates = []
                    final_result.important_dates.extend(res.important_dates)

        return final_result

    async def extract_university_info(self, chunks: List[TextChunk]) -> UniversityInfo:
        """Extract university name, short name, and location."""
        # Look for chunks mentioning university name - check first 30 chunks
        uni_chunks = [
            c for c in chunks[:30]
            if "university" in c.text.lower() or "college" in c.text.lower()
        ]
        if not uni_chunks:
            uni_chunks = chunks[:5]
        
        context_text = "\n\n".join([c.text for c in uni_chunks[:5]])
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
                            "content": f"{PROMPTS['university_info']}\n\nDATA:\n{context_text}"
                        }
                    ],
                    max_retries=3,
                )
        except Exception as e:
            logger.error(f"Failed to extract university info: {type(e).__name__}: {e}")
            return UniversityInfo()

    async def extract_all(self, chunks: List[TextChunk]) -> UniversityExtraction:
        """Main extraction pipeline with generic prompts."""
        logger.info(f"Starting extraction on {len(chunks)} chunks")
        
        # Log section distribution
        section_counts = {}
        for c in chunks:
            label = c.section_label or "unknown"
            section_counts[label] = section_counts.get(label, 0) + 1
        logger.info(f"Chunk section distribution: {section_counts}")

        # Step 1: University Info
        logger.info("Step 1/5: Extracting university info...")
        try:
            uni_info = await self.extract_university_info(chunks)
        except Exception as e:
            logger.error(f"University info extraction failed: {e}")
            uni_info = UniversityInfo()

        # Step 2: Departments and Programs
        logger.info("Step 2/5: Extracting departments and programs...")
        try:
            dept_data = await self._extract_section(
                chunks, 
                DepartmentList, 
                primary_tags=["departments"],
                fallback_tags=["programs", "curriculum", "general"],
                prompt_instruction=PROMPTS["departments"],
                max_chunks=150  # More for departments since there are many
            )
        except Exception as e:
            logger.error(f"Department extraction failed: {e}")
            dept_data = DepartmentList()
        
        # Step 3: Facilities
        logger.info("Step 3/5: Extracting facilities...")
        try:
            fac_data = await self._extract_section(
                chunks, 
                FacilityList, 
                primary_tags=["facilities"],
                fallback_tags=["departments", "general"],
                prompt_instruction=PROMPTS["facilities"],
                max_chunks=90
            )
        except Exception as e:
            logger.error(f"Facility extraction failed: {e}")
            fac_data = FacilityList()
        
        # Step 4: Fees
        logger.info("Step 4/5: Extracting fees...")
        try:
            fee_data = await self._extract_section(
                chunks, 
                FeeList, 
                primary_tags=["fees"],
                fallback_tags=["admissions", "general"],
                prompt_instruction=PROMPTS["fees"],
                max_chunks=60
            )
        except Exception as e:
            logger.error(f"Fee extraction failed: {e}")
            fee_data = FeeList()
        
        # Step 5: Admissions
        logger.info("Step 5/5: Extracting admissions...")
        try:
            admissions = await self._extract_section(
                chunks, 
                AdmissionInfo, 
                primary_tags=["admissions"],
                fallback_tags=["requirements", "general"],
                prompt_instruction=PROMPTS["admissions"],
                max_chunks=60
            )
        except Exception as e:
            logger.error(f"Admission extraction failed: {e}")
            admissions = None

        # Log extraction results
        logger.info(f"Extracted university: {uni_info.name}")
        logger.info(f"Extracted {len(dept_data.items)} departments")
        logger.info(f"Extracted {len(fac_data.items)} facilities")
        logger.info(f"Extracted {len(fee_data.items)} fees")

        # Build metadata with realistic confidence scores
        dept_confidence = min(0.9, 0.3 + (len(dept_data.items) * 0.03)) if dept_data.items else 0.0
        fac_confidence = min(0.9, 0.3 + (len(fac_data.items) * 0.05)) if fac_data.items else 0.0
        fee_confidence = min(0.9, 0.3 + (len(fee_data.items) * 0.1)) if fee_data.items else 0.0
        adm_confidence = 0.7 if admissions and admissions.eligibility_criteria else 0.0

        metadata = ExtractionMetaData(
            extraction_timestamp=datetime.now(),
            total_chunks_processed=len(chunks),
            total_pages=max(c.page_number for c in chunks) if chunks else 0,
            confidence_scores={
                "departments": dept_confidence,
                "facilities": fac_confidence,
                "fees": fee_confidence,
                "admissions": adm_confidence,
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