import sys
import os
sys.path.insert(0, os.path.abspath(os.path.join(os.path.dirname(__file__), '../')))

from src.services.chunker import chunker_service, TextChunk
from src.services.document_parser import ParsedPage, ParsedDocument
from src.models.db import ChunkType

def test_chunk_categorization():
    print("=== Testing Chunker Categorization Logic ===")
    
    # Test Case 1: Simple string with "Department"
    text1 = "The Department of Computer Science offers various programs."
    chunk1 = chunker_service._create_chunk(text1, ChunkType.PARAGRAPH, ParsedPage(page_number=1, text="", tables=[]), "General", 0)
    print(f"Text: '{text1}'\nExpected: Departments | Got: {chunk1.section_label}\n")

    # Test Case 2: String with "Fee"
    text2 = "Tuition Fee per semester is 50,000 PKR."
    chunk2 = chunker_service._create_chunk(text2, ChunkType.PARAGRAPH, ParsedPage(page_number=1, text="", tables=[]), "General", 0)
    print(f"Text: '{text2}'\nExpected: Fees | Got: {chunk2.section_label}\n")
    
    # Test Case 3: Verify Keywords
    print("Checking Keywords for 'departments':", chunker_service.SECTION_KEYWORDS['departments'])

if __name__ == "__main__":
    test_chunk_categorization()
