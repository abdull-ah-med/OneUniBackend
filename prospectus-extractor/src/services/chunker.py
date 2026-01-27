import re
import uuid
import logging
from typing import List, Optional, Tuple
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
    # Regex patterns for section detection (more reliable than keywords)
    SECTION_PATTERNS = {
        "departments": [
            r"DEPARTMENT\s+OF\s+[A-Z\s&]+",
            r"Faculty\s+of\s+[A-Za-z\s&]+",
            r"FACULTIES\s*&\s*DEPARTMENTS",
        ],
        "programs": [
            r"B\.?Sc\.?\s+[A-Za-z\s]+",
            r"BS\s+[A-Za-z\s]+",
            r"Bachelor\s+of\s+[A-Za-z\s]+",
            r"M\.?Sc\.?\s+[A-Za-z\s]+",
            r"Ph\.?D\.?\s+[A-Za-z\s]+",
            r"Courses?\s+of\s+Study",
            r"Undergraduate\s+Programs?",
            r"Postgraduate\s+Programs?",
        ],
        "curriculum": [
            r"Year\s+\d",
            r"Semester\s+\d",
            r"Credit\s+Hours?",
            r"Course\s+No",
        ],
        "fees": [
            r"Fee\s+Structure",
            r"Tuition\s+Fee",
            r"Rs\.?\s*[\d,]+",
            r"Payment\s+Schedule",
            r"Financial\s+Aid",
            r"Scholarship",
        ],
        "admissions": [
            r"Admission\s+(?:Criteria|Requirements?|Eligibility|Process)",
            r"Eligibility\s+Criteria",
            r"How\s+to\s+Apply",
            r"Application\s+(?:Process|Procedure|Form)",
            r"Entry\s+Test",
            r"Entrance\s+(?:Exam|Test)",
            r"Merit\s+(?:List|Criteria)",
        ],
        "facilities": [
            r"Laborator(?:y|ies)",
            r"Library",
            r"Hostel",
            r"Sports\s+Facilit",
            r"Campus\s+Facilit",
            r"Research\s+(?:Center|Lab)",
        ],
        "contact": [
            r"Contact\s+(?:Us|Information|Details)",
            r"Address\s*:",
            r"Phone\s*:",
            r"Email\s*:",
            r"Website\s*:",
        ],
    }

    # Patterns to identify major document headers
    HEADER_PATTERNS = [
        r"^DEPARTMENT\s+OF\s+[A-Z\s&]+$",
        r"^FACULTY\s+OF\s+[A-Z\s&]+$",
        r"^B\.?Sc\.?\s+[A-Za-z\s]+$",
        r"^BS\s+[A-Za-z\s]+$",
        r"^[A-Z][A-Z\s&]+$",  # All caps headers
    ]

    # Noise patterns to remove (generic for any university)
    NOISE_PATTERNS = [
        r"(?:Undergraduate|Postgraduate|Graduate)\s+Prospectus\s+(?:Spring|Fall|Summer)?\s*\d+",
        r"^\d+\s*$",  # Standalone page numbers
        r"^www\.[a-z.-]+\.edu\.[a-z]+\s*$",  # Any university website
        r"^Page\s*\d+\s*(?:of\s*\d+)?\s*$",  # Page numbers
    ]

    def __init__(self):
        self.chunk_size = settings.chunk_size
        self.chunk_overlap = max(settings.chunk_overlap, 300)  # Minimum 300 for context

    def chunk_document(self, document: ParsedDocument) -> List[TextChunk]:
        """Main chunking pipeline with preprocessing."""
        all_chunks: List[TextChunk] = []
        current_section = "general"
        current_header = None
        position = 0

        for page in document.pages:
            # Process tables first
            for table in page.tables:
                table_text = self._format_table(table)
                if table_text:
                    section = self._classify_section(table_text, current_section)
                    all_chunks.append(self._create_chunk(
                        text=table_text,
                        chunk_type=ChunkType.TABLE,
                        page=page,
                        section=section,
                        position=position,
                        header_context=current_header
                    ))
                    position += 1

            # Preprocess page text to remove noise
            cleaned_text = self._preprocess_text(page.text)
            
            # Split into blocks
            raw_blocks = re.split(r'\n\s*\n', cleaned_text)
            buffer_text = ""

            for block in raw_blocks:
                block = block.strip()
                if not block:
                    continue

                # Check if this is a major header
                header_match = self._detect_header(block)
                if header_match:
                    # Flush buffer before starting new section
                    if buffer_text:
                        section = self._classify_section(buffer_text, current_section)
                        all_chunks.append(self._create_chunk(
                            buffer_text, ChunkType.PARAGRAPH, page, section, position, current_header
                        ))
                        buffer_text = ""
                        position += 1

                    current_header = header_match
                    current_section = self._classify_section(block, "general")
                    all_chunks.append(self._create_chunk(
                        block, ChunkType.HEADING, page, current_section, position, current_header
                    ))
                    position += 1
                    continue

                # Check if block is a curriculum table (pipe-delimited)
                if self._is_table_format(block):
                    section = self._classify_section(block, current_section)
                    if "credit" in block.lower() or "semester" in block.lower():
                        section = "curriculum"
                    all_chunks.append(self._create_chunk(
                        block, ChunkType.TABLE, page, section, position, current_header
                    ))
                    position += 1
                    continue

                # Regular text accumulation with size limit
                if len(buffer_text) + len(block) <= self.chunk_size:
                    buffer_text = f"{buffer_text}\n\n{block}" if buffer_text else block
                else:
                    if buffer_text:
                        section = self._classify_section(buffer_text, current_section)
                        all_chunks.append(self._create_chunk(
                            buffer_text, ChunkType.PARAGRAPH, page, section, position, current_header
                        ))
                        position += 1
                        buffer_text = self._get_smart_overlap(buffer_text)

                    if len(block) > self.chunk_size:
                        sub_chunks = self._split_large_text(block, page, current_section, position, current_header)
                        all_chunks.extend(sub_chunks)
                        position += len(sub_chunks)
                        buffer_text = self._get_smart_overlap(block)
                    else:
                        buffer_text = f"{buffer_text}\n\n{block}" if buffer_text else block

            # Flush page buffer
            if buffer_text:
                section = self._classify_section(buffer_text, current_section)
                all_chunks.append(self._create_chunk(
                    buffer_text, ChunkType.PARAGRAPH, page, section, position, current_header
                ))
                position += 1

        logger.info(f"Generated {len(all_chunks)} chunks from {document.total_pages} pages")
        return all_chunks

    def _preprocess_text(self, text: str) -> str:
        """Remove PDF noise and normalize text."""
        # Remove noise patterns
        for pattern in self.NOISE_PATTERNS:
            text = re.sub(pattern, "", text, flags=re.MULTILINE | re.IGNORECASE)

        # Remove duplicate consecutive lines (common PDF artifact)
        lines = text.split('\n')
        cleaned_lines = []
        prev_line = None
        for line in lines:
            stripped = line.strip()
            # Skip if this line is a near-duplicate of the previous
            if prev_line and self._is_near_duplicate(stripped, prev_line):
                continue
            cleaned_lines.append(line)
            if stripped:
                prev_line = stripped

        text = '\n'.join(cleaned_lines)

        # Collapse excessive whitespace but preserve paragraph breaks
        text = re.sub(r'\n{4,}', '\n\n\n', text)
        text = re.sub(r'[ \t]+', ' ', text)

        return text.strip()

    def _is_near_duplicate(self, line1: str, line2: str) -> bool:
        """Check if two lines are near-duplicates (common in PDF layout mode)."""
        if not line1 or not line2:
            return False
        # Check if one is a substring of the other (common PDF artifact)
        if len(line1) < 10 or len(line2) < 10:
            return False
        # One line is contained in the other
        if line1 in line2 or line2 in line1:
            return True
        # High similarity ratio
        shorter, longer = (line1, line2) if len(line1) < len(line2) else (line2, line1)
        if len(shorter) > 20 and shorter[:20] == longer[:20]:
            return True
        return False

    def _detect_header(self, text: str) -> Optional[str]:
        """Detect if text is a major document header."""
        text = text.strip()
        if len(text) > 150:  # Headers aren't that long
            return None
        
        for pattern in self.HEADER_PATTERNS:
            if re.match(pattern, text, re.IGNORECASE):
                return text

        # Also check for structured headers like "DEPARTMENT OF COMPUTER SCIENCE"
        if re.match(r"^DEPARTMENT\s+OF\s+", text, re.IGNORECASE):
            return text
        if re.match(r"^B\.?S\.?c?\.\s+", text, re.IGNORECASE):
            return text

        return None

    def _is_table_format(self, text: str) -> bool:
        """Check if text looks like a formatted table."""
        lines = text.strip().split('\n')
        if len(lines) < 2:
            return False
        # Count lines with pipe delimiters
        pipe_lines = sum(1 for line in lines if '|' in line)
        return pipe_lines >= len(lines) * 0.5

    def _classify_section(self, text: str, current_section: str) -> str:
        """Classify text into a section using pattern matching."""
        text_sample = text[:1500]  # Check first part of text

        # Score each section based on pattern matches
        scores = {}
        for section, patterns in self.SECTION_PATTERNS.items():
            score = 0
            for pattern in patterns:
                matches = re.findall(pattern, text_sample, re.IGNORECASE)
                score += len(matches)
            if score > 0:
                scores[section] = score

        if scores:
            # Return section with highest score
            best_section = max(scores, key=scores.get)
            if scores[best_section] >= 1:
                return best_section

        # Fall back to current section if no strong match
        return current_section

    def _split_large_text(self, text: str, page: ParsedPage, section: str, position: int, header_context: Optional[str]) -> List[TextChunk]:
        """Safely splits a massive block of text that exceeds chunk_size."""
        chunks = []
        while len(text) > self.chunk_size:
            cut_index = self._find_safe_cut_index(text, self.chunk_size)
            chunk_text = text[:cut_index].strip()
            chunks.append(self._create_chunk(chunk_text, ChunkType.PARAGRAPH, page, section, position, header_context))
            overlap_text = self._get_smart_overlap(chunk_text)
            text = overlap_text + text[cut_index:]
            position += 1

        if text.strip():
            chunks.append(self._create_chunk(text, ChunkType.PARAGRAPH, page, section, position, header_context))
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

    def _create_chunk(self, text: str, chunk_type: ChunkType, page: ParsedPage, section: str, position: int, header_context: Optional[str] = None) -> TextChunk:
        """Create a chunk with proper section labeling."""
        # Ensure section is lowercase for consistent matching
        section = section.lower() if section else "general"
        
        metadata = {
            "length": len(text),
            "page": page.page_number
        }
        if header_context:
            metadata["parent_header"] = header_context

        return TextChunk(
            chunk_id=str(uuid.uuid4()),
            text=text.strip(),
            chunk_type=chunk_type,
            page_number=page.page_number,
            position_in_doc=position,
            section_label=section,
            metadata=metadata
        )

    def _format_table(self, table: List[List[str]]) -> str:
        """Format table as pipe-delimited text."""
        if not table:
            return ""
        lines = []
        for row in table:
            cleaned_row = [str(cell).replace('\n', ' ').strip() for cell in row if cell]
            if cleaned_row:
                lines.append("| " + " | ".join(cleaned_row) + " |")
        return "\n".join(lines)


chunker_service = ChunkerService()