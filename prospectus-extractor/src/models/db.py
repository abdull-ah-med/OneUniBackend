from sqlalchemy import (Column, String, Text, Integer, DateTime, ForeignKey, Enum, Numeric, func, Index, create_engine)
from sqlalchemy.dialects.postgresql import UUID, JSONB
from sqlalchemy.orm import declarative_base, relationship, sessionmaker
from pgvector.sqlalchemy import Vector
import enum
import uuid
Base = declarative_base()

class IngestionStatus(str, enum.Enum):
    PENDING = "pending"
    PROCESSING = "processing"
    COMPLETED = "completed"
    FAILED = "failed"
class ChunkType(str, enum.Enum):
    HEADING = "heading"
    PARAGRAPH = "paragraph"
    TABLE = "table"
    LIST = "list"
class ProspectusIngestion(Base):
    __tablename__ = "prospectus_ingestions"
    ingestion_id = Column(UUID(as_uuid=True), primary_key=True, default=uuid.uuid4)
    university_id = Column(UUID(as_uuid=True), ForeignKey("universities.university_id"), nullable=True)
    blob_url = Column(Text, nullable=False)
    file_name = Column(Text, nullable=False)
    mime_type = Column(String(100))
    file_size_bytes = Column(Integer)

    status = Column(String(20), default=IngestionStatus.PENDING.value, nullable=False)
    error_message = Column(Text)
    retry_count = Column(Integer, default=0)

    created_at = Column(DateTime(timezone=True), server_default=func.now())
    updated_at = Column(DateTime(timezone=True), server_default=func.now(), onupdate=func.now())
    processing_started_at = Column(DateTime(timezone=True))
    completed_at = Column(DateTime(timezone=True))

    extractions = relationship("ProspectusExtraction", back_populates="ingestion", cascade="all, delete-orphan")
    chunks = relationship("ProspectusChunk", back_populates="ingestion", cascade="all, delete-orphan")

class ProspectusExtraction(Base):
    __tablename__ = "prospectus_extractions"
    extraction_id = Column(UUID(as_uuid=True), primary_key=True, default = uuid.uuid4)
    ingestion_id = Column(UUID(as_uuid=True), ForeignKey("prospectus_ingestions.ingestion_id"), nullable=False)
    schema_version = Column(String(20), default="v1.0.0", nullable=False)
    extracted_json=Column(JSONB, nullable=False)
    confidence_scores=Column(Numeric(3, 2))
    total_entities_extracted=Column(Integer, default=0)
    created_at = Column(DateTime(timezone=True), server_default=func.now())
    ingestion = relationship("ProspectusIngestion", back_populates="extractions")

class ProspectusChunk(Base):
    __tablename__ = "prospectus_chunks"
    chunk_id = Column(UUID(as_uuid=True), primary_key=True, default=uuid.uuid4)
    ingestion_id = Column(UUID(as_uuid=True), ForeignKey("prospectus_ingestions.ingestion_id"), nullable=False)
    chunk_type = Column(String(20))
    chunk_text = Column(Text, nullable=False)
    page_number = Column(Integer)
    position_in_doc = Column(Integer)
    section_label = Column(String(50))
    created_at = Column(DateTime(timezone=True), server_default = func.now())
    ingestion = relationship("ProspectusIngestion", back_populates="chunks")
    vector = relationship("ProspectusVector", back_populates="chunk", uselist=False, cascade="all, delete-orphan")
    __table_args__ = (
        Index("idx_chunks_ingestion_id", "ingestion_id"),
        Index("idx_chunks_section_label", "section_label"),
    )
class ProspectusVector(Base):
    __tablename__= "prospectus_vectors"
    vector_id=Column(UUID(as_uuid=True), primary_key=True, default=uuid.uuid4)
    chunk_id=Column(UUID(as_uuid=True), ForeignKey("prospectus_chunks.chunk_id"), nullable=False, unique=True)
    embedding = Column(Vector(1024), nullable=False)
    created_at = Column(DateTime(timezone=True), server_default = func.now())
    chunk = relationship("ProspectusChunk", back_populates="vector")


def get_engine(database_url:str):
    return create_engine(database_url)

def get_session_factory(engine):
    return sessionmaker(bind=engine)
