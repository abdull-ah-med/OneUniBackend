import sys
import os
import asyncio
import logging

# Add project root to python path to allow imports from src
# Current file is in scripts/test/
# ../../ resolves to project root
sys.path.insert(0, os.path.abspath(os.path.join(os.path.dirname(__file__), '../../')))

from src.services.chunker import TextChunk
from src.models.db import ChunkType
from src.services.llm_client import extraction_service

logging.basicConfig(level=logging.INFO)

async def test_llm():
    # Mock Chunks
    chunks = [
        TextChunk(
            chunk_id="1",
            text="Welcome to the University of Technology. We are located in Tech City, USA.",
            chunk_type=ChunkType.PARAGRAPH,
            page_number=1,
            section_label="General"
        ),
        TextChunk(
            chunk_id="2",
            text="The Department of Computer Science offers a B.S. in AI and M.S. in Data Science.",
            chunk_type=ChunkType.PARAGRAPH,
            page_number=2,
            section_label="Departments"
        ),
         TextChunk(
            chunk_id="3",
            text="Tuition is $500 per credit. Housing fees are $2000 per semester.",
            chunk_type=ChunkType.PARAGRAPH,
            page_number=3,
            section_label="Fees"
        )
    ]
    
    print("🚀 Starting LLM Extraction Test using Local Ollama...")
    try:
        result = await extraction_service.extract_all(chunks)
        
        print("\n✅ Extraction Successful!")
        print(f"University: {result.university_name}")
        print(f"Location: {result.location}")
        
        print(f"\nDepartments ({len(result.departments)}):")
        for d in result.departments:
            print(f" - {d.name}")
            
        print(f"\nFees ({len(result.fee_structure)}):")
        for f in result.fee_structure:
            print(f" - {f.program_name or 'General'}: {f.fee_type} ({f.amount} {f.currency})")
            
    except Exception as e:
        print(f"\n❌ Test Failed: {e}")
        print("Ensure Ollama is running: `ollama serve` and `ollama pull llama3.1:8b`")

if __name__ == "__main__":
    asyncio.run(test_llm())
