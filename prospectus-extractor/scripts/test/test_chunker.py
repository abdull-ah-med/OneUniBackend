import asyncio
import sys
import os
import logging
from datetime import datetime

# Add src to python path
sys.path.insert(0, os.path.abspath(os.path.join(os.path.dirname(__file__), '../src')))

from services.document_parser import document_parser_service
from services.chunker import chunker_service
from services.blob_storage import blob_storage # Re-using for download

# Configure logging
logging.basicConfig(level=logging.INFO, format='%(name)s - %(levelname)s - %(message)s')
logger = logging.getLogger("test_chunker")

async def test_chunking():
    # Use the blob we uploaded in the previous test (or re-upload if needed, but let's assume local file for speed or re-upload)
    # Actually, let's just use the local file directly for parser -> chunker speed,
    # unless we want to verify the full flow again. 
    # For unit testing chunker, local parse is faster.
    
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
    headings = [c for c in chunks if c.chunk_type == "heading"] # String match or Enum match depending on repr
    # Enum is 'heading' value
    
    from models.db import ChunkType
    headings = [c for c in chunks if c.chunk_type == ChunkType.HEADING]
    paragraphs = [c for c in chunks if c.chunk_type == ChunkType.PARAGRAPH]
    lists = [c for c in chunks if c.chunk_type == ChunkType.LIST]
    
    print(f"Headings: {len(headings)}")
    print(f"Paragraphs: {len(paragraphs)}")
    print(f"Lists: {len(lists)}")
    print(f"Other/Tables (if any): {len(chunks) - len(headings) - len(paragraphs) - len(lists)}")

    print("\n" + "-"*30)
    print("🔍 SAMPLE CHUNKS (First 10)")
    print("-" * 30)
    for i, chunk in enumerate(chunks[:10]):
        print(f"[{i+1}] Type: {chunk.chunk_type.value} | P.{chunk.page_number} | Label: {chunk.section_label}")
        print(f"Content: {chunk.text[:100]}...") # First 100 chars
        print("-" * 15)

if __name__ == "__main__":
    asyncio.run(test_chunking())
