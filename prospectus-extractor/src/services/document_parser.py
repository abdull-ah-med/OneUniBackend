import io
import asyncio
import logging
import pdfplumber
from dataclasses import dataclass, field
from typing import List, Optional

logger = logging.getLogger(__name__)

@dataclass
class ParsedPage:
    page_number: int
    text: str
    tables: List[List[List[str]]] = field(default_factory=list)

@dataclass
class ParsedDocument:
    total_pages:int
    pages: List[ParsedPage]
    metadata: dict = field(default_factory=dict)

    @property
    def full_text(self) -> str:
        "Helper to get text from all pages joined"
        parts = [f"Page {p.page_number}: {p.text}" for p in self.pages]
        return "\n\n".join(parts)

class DocumentParserService:
    async def parse_pdf(self, pdf_bytes: bytes) -> ParsedDocument:
        """ aync parse PDF using thread pool to avoid blocking the event loop"""
        logger.info(f"Starting PDF parsing for {len(pdf_bytes)} bytes")
        try:
            return await asyncio.to_thread(self._parse_sync, pdf_bytes)
        except Exception as e:
            logger.error(f"Error parsing PDF: {str(e)}")
            raise
    def _parse_sync(self, pdf_bytes:bytes) ->ParsedDocument:
        pages=[]
        with pdfplumber.open(io.BytesIO(pdf_bytes)) as pdf:
            metadata = pdf.metadata or {}
            total_pages = len(pdf.pages)

            for i, page in enumerate(pdf.pages):
                text = page.extract_text(layout=True) or ""
                tables = page.extract_tables() or []
                cleaned_tables = [[[cell or "" for cell in row] for row in table] for table in tables]
                pages.append(ParsedPage(page_number = i + 1, text=text, tables=cleaned_tables))
        return ParsedDocument(total_pages=total_pages, pages=pages, metadata=metadata)
document_parser_service = DocumentParserService()