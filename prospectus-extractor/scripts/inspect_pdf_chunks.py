import sys
import os
import asyncio
import logging

sys.path.insert(0, os.path.abspath(os.path.join(os.path.dirname(__file__), '../')))

from src.services.document_parser import document_parser_service
from src.services.chunker import chunker_service

logging.basicConfig(level=logging.INFO)

async def inspect():
    pdf_path = "data/UG_WHOLE_Spring_26_08_12_2025.pdf"
    if not os.path.exists(pdf_path):
        print("File not found")
        return

    print("Parsing...")
    with open(pdf_path, "rb") as f:
        pdf_bytes = f.read()
    parsed = await document_parser_service.parse_pdf(pdf_bytes)
    
    print("Chunking...")
    chunks = chunker_service.chunk_document(parsed)
    
    dept_chunks = [c for c in chunks if c.section_label and c.section_label.lower() in ["departments", "programs", "faculties"]]
    
    print(f"Total Chunks: {len(chunks)}")
    print(f"Dept Chunks: {len(dept_chunks)}")
    
    with open("scripts/test/dept_chunks_dump.txt", "w") as f:
        for i, c in enumerate(dept_chunks):
            f.write(f"\n--- CHUNK {i} [Page {c.page_number}] [Label: {c.section_label}] ---\n")
            f.write(c.text)
            f.write("\n" + "="*50 + "\n")
            
    print("Dumped to scripts/test/dept_chunks_dump.txt")

if __name__ == "__main__":
    asyncio.run(inspect())
