from locale import currency
from re import S
from pydantic import BaseModel, Field
from decimal import Decimal
from typing import Optional
from datetime import datetime

class Program(BaseModel):
    name: str
    degree_type: Optional[str] = None
    duration_years: Optional[int] = None
    credit_hours: Optional[int] = None
    eligibility_criteria: Optional[str] = None
    description: Optional[str] = None
    source_chunk_ids: list[str] = Field(default_factory=list)

class Department(BaseModel):
    name: str
    short_name: Optional[str] = None
    head_of_department: Optional[str] = None
    email: Optional[str] = None
    phone: Optional[str] = None
    description: Optional[str] = None
    programs: list[Program] = Field(default_factory=list)
    source_chunk_ids: list[str] = Field(default_factory=list)

class Facility(BaseModel):
    name: str
    facility_type: Optional[str] = None
    description: Optional[str] = None
    capacity: Optional[int] = None
    location: Optional[str] = None
    source_chunk_ids: list[str] = Field(default_factory=list)

class FeeItem(BaseModel):
    program_name: Optional[str] = None
    fee_type: Optional[str] = None
    amount: Optional[Decimal] = None
    frequency: Optional[str] = None
    currency: str = "PKR"
    source_chunk_ids: list[str] = Field(default_factory=list)

class AdmissionInfo(BaseModel):
    eligibility_criteria: Optional[str] = None
    application_process:Optional[str] = None
    test_requirements: list[str] = Field(default_factory=list)
    documents_required: list[str] = Field(default_factory=list)
    important_dates: list[str] = Field(default_factory=list)
    source_chunk_ids: list[str] = Field(default_factory=list)
class ContactInfo(BaseModel):
    address: Optional[str] = None
    phone: Optional[str] = None
    email: Optional[str] = None
    website: Optional[str] = None
    social_media: dict[str, str] = Field(default_factory = dict)

class ExtractionMetaData(BaseModel):
    extraction_timestamp: datetime = Field(default_factory=datetime.now)
    total_chunks_processed: int = 0
    total_pages: int = 0
    confidence_scores: dict[str, float] = Field(default_factory=dict)
    warnings:list[str] = Field(default_factory=list)

class UniversityExtraction(BaseModel):
    schema_version: str = "v1.0.0"
    university_name: str
    university_short_name: Optional[str] = None
    location: Optional[str] = None

    departments: list[Department] = Field(default_factory=list)
    facilities: list[Facility] = Field(default_factory=list)
    fee_structure: list[FeeItem] = Field(default_factory=list)
    admissions: Optional[AdmissionInfo] = None
    contact: Optional[ContactInfo] = None

    metadata: ExtractionMetaData = Field(default_factory=ExtractionMetaData)