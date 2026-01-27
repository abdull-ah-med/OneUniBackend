import asyncio
import sys
import os
import logging
import json
from datetime import datetime

# Add project root to python path (parent of src)
sys.path.insert(0, os.path.abspath(os.path.join(os.path.dirname(__file__), '..')))

from src.services.document_parser import document_parser_service
from src.services.chunker import chunker_service

# Configure logging
logging.basicConfig(level=logging.INFO, format='%(name)s - %(levelname)s - %(message)s')
logger = logging.getLogger("test_chunker")

async def test_chunking():
    pdf_path = "data/UG_WHOLE_Spring_26_08_12_2025.pdf"
    if not os.path.exists(pdf_path):
        logger.error(f"File not found: {pdf_path}")
        return

    print(f"\n[{datetime.now().time()}] === 1. PARSING LOCAL PDF ===")
    with open(pdf_path, "rb") as f:
        pdf_bytes = f.read()
    
    parsed_doc = await document_parser_service.parse_pdf(pdf_bytes)
    print(f"Parsed {parsed_doc.total_pages} pages.")

    print(f"\n[{datetime.now().time()}] === 2. CHUNKING DOCUMENT ===")
    start_time = datetime.now()
    chunks = chunker_service.chunk_document(parsed_doc)
    duration = datetime.now() - start_time
    
    print(f"✅ Chunked in {duration.total_seconds():.2f} seconds!")
    print(f"Total Chunks: {len(chunks)}")
    
    # Stats
    from src.models.db import ChunkType
    headings = [c for c in chunks if c.chunk_type == ChunkType.HEADING]
    paragraphs = [c for c in chunks if c.chunk_type == ChunkType.PARAGRAPH]
    tables = [c for c in chunks if c.chunk_type == ChunkType.TABLE]
    lists = [c for c in chunks if c.chunk_type == ChunkType.LIST]
    
    print(f"Headings: {len(headings)}")
    print(f"Paragraphs: {len(paragraphs)}")
    print(f"Tables: {len(tables)}")
    print(f"Lists: {len(lists)}")
    
    # Dump chunks in exact format passed to LLM (from llm_client.py line 55)
    # context_text = "\n\n".join([c.text for c in chunks])
    print(f"\n[{datetime.now().time()}] === 3. DUMPING CHUNKS (LLM FORMAT) ===")
    output_path = "data/chunks_llm_format.txt"
    
    # Exact format used in llm_client.py _extract_batch method
    context_text = "\n\n".join([c.text for c in chunks])
    
    with open(output_path, "w", encoding="utf-8") as f:
        f.write(context_text)
    
    print(f"✅ Dumped {len(chunks)} chunks to: {output_path}")
    print(f"   Total characters: {len(context_text)}")

if __name__ == "__main__":
    asyncio.run(test_chunking())
