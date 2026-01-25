import re
import uuid
import logging
from typing import List, Optional
from dataclasses import dataclass, field

from src.config import settings
from src.models.db import ChunkType
from src.services.document_parser import ParsedPage, ParsedDocument

logger = logging.getLogger(__name__)

@dataclass
class TextChunk:
    chunk_id: str
    text: str
    chunk_type: ChunkType
    page_number: int
    position_in_doc: int = 0
    section_label: Optional[str] = None
    metadata: dict = field(default_factory=dict)

class ChunkerService:
    SECTION_KEYWORDS = {
        "departments": ["department", "faculty", "school", "college", "institute"],
        "programs": ["program", "degree", "bachelor", "master", "phd", "bs", "ms", "course", "curriculum"],
        "fees": ["fee", "tuition", "cost", "payment", "scholarship", "financial"],
        "facilities": ["facility", "library", "hostel", "lab", "laboratory", "sports", "cafeteria", "campus"],
        "admissions": ["admission", "eligibility", "requirement", "apply", "application", "deadline", "entry"],
        "contact": ["contact", "address", "phone", "email", "website", "location"],
        "policies": ["policy", "procedure", "rule", "regulation", "guideline", "policy", "procedure", "rule", "regulation", "guideline"],
        "requirements": ["requirement", "eligibility", "admission", "application", "deadline", "entry", "criteria", "standard", "policy", "procedure", "rule", "regulation", "guideline"],
    }
    def __init__(self):
        self.chunk_size = settings.chunk_size
        self.chunk_overlap = settings.chunk_overlap

    def chunk_document(self, document: ParsedDocument) -> List[TextChunk]:
        all_chunks: List[TextChunk] = []
        current_section = "general"
        position = 0
        for page in document.pages:
            for table in page.tables:
                table_text = self._format_table(table)
                if table_text:
                    all_chunks.append(self._create_chunk(text=table_text, chunk_type=ChunkType.TABLE, page = page, section = current_section, position=position))
                    position += 1
            raw_blocks = re.split(r'\n\s*\n', page.text)
            buffer_text = ""
            
            for block in raw_blocks:
                block = block.strip()
                if not block: 
                    continue

                if self._is_heading(block):
                    if buffer_text:
                        all_chunks.append(self._create_chunk(buffer_text, ChunkType.PARAGRAPH, page, current_section, position))
                        buffer_text = ""
                        position += 1
                    
                    current_section = self._clean_heading(block)
                    all_chunks.append(self._create_chunk(block, ChunkType.HEADING, page, current_section, position))
                    position += 1
                    continue
                
                if len(buffer_text) + len(block) <= self.chunk_size:
                    buffer_text = f"{buffer_text}\n\n{block}" if buffer_text else block
                else:
                    if buffer_text:
                        all_chunks.append(self._create_chunk(buffer_text, ChunkType.PARAGRAPH, page, current_section, position))
                        position += 1
                        buffer_text = self._get_smart_overlap(buffer_text)

                    if len(block) > self.chunk_size:
                        sub_chunks = self._split_large_text(block, page, current_section, position)
                        all_chunks.extend(sub_chunks)
                        position += len(sub_chunks)
                        buffer_text = self._get_smart_overlap(block)
                    else:
                        buffer_text = f"{buffer_text}\n\n{block}" if buffer_text else block
            
            # Flush page buffer
            if buffer_text:
                all_chunks.append(self._create_chunk(buffer_text, ChunkType.PARAGRAPH, page, current_section, position))
                position += 1
                
        logger.info(f"Generated {len(all_chunks)} chunks from {document.total_pages} pages")
        return all_chunks

    def _split_large_text(self, text: str, page: ParsedPage, section: str, position: int) -> List[TextChunk]:
        """Safely splits a massive block of text that exceeds chunk_size."""
        chunks = []
        while len(text) > self.chunk_size:
            cut_index = self._find_safe_cut_index(text, self.chunk_size)
            chunk_text = text[:cut_index].strip()
            chunks.append(self._create_chunk(chunk_text, ChunkType.PARAGRAPH, page, section, position))
            overlap_text = self._get_smart_overlap(chunk_text)
            text = overlap_text + text[cut_index:]
            position += 1
            
        if text.strip():
            chunks.append(self._create_chunk(text, ChunkType.PARAGRAPH, page, section, position))
        return chunks

    def _find_safe_cut_index(self, text: str, max_limit: int) -> int:
        """Finds the best split point (period > newline > space) before max_limit."""
        if len(text) <= max_limit:
            return len(text)
        search_area = text[:max_limit]
        match = list(re.finditer(r'[.!?]\s', search_area))
        if match:
            return match[-1].end()
        last_newline = search_area.rfind('\n')
        if last_newline != -1:
            return last_newline + 1
        last_space = search_area.rfind(' ')
        if last_space != -1:
            return last_space + 1
        return max_limit

    def _get_smart_overlap(self, text: str) -> str:
        """Returns the last N chars, but snapped to the nearest sentence start."""
        if len(text) <= self.chunk_overlap:
            return text
        raw_overlap = text[-self.chunk_overlap:]
        match = re.search(r'[.!?]\s+', raw_overlap)
        if match:
            return raw_overlap[match.end():]
        return raw_overlap

    def _create_chunk(self, text, chunk_type, page, section, position):
        if section == "General":
            text_lower = text.lower()
            for key, keywords in self.SECTION_KEYWORDS.items():
                if any(k in text_lower for k in keywords):
                    section = key.capitalize()
                    break
        return TextChunk(
            chunk_id=str(uuid.uuid4()),
            text=text.strip(),
            chunk_type=chunk_type,
            page_number=page.page_number,
            position_in_doc=position,
            section_label=section,
            metadata={"length": len(text)}
        )

    def _format_table(self, table: List[List[str]]) -> str:
        if not table: return ""
        lines = []
        for row in table:
            cleaned_row = [str(cell).replace('\n', ' ').strip() for cell in row if cell]
            if cleaned_row:
                lines.append("| " + " | ".join(cleaned_row) + " |")
        return "\n".join(lines)

    def _is_heading(self, text: str) -> bool:
        return len(text) < 100 and not text.endswith('.') and (text.istitle() or text.isupper())

    def _clean_heading(self, text: str) -> str:
        return re.sub(r'\s+', ' ', text).strip()

chunker_service = ChunkerService()