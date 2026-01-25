import sys
import os
import asyncio
import logging
from datetime import datetime

# Add project root to python path
sys.path.insert(0, os.path.abspath(os.path.join(os.path.dirname(__file__), '../../')))

from src.services.document_parser import document_parser_service
from src.services.chunker import chunker_service
from src.services.llm_client import extraction_service

logging.basicConfig(level=logging.INFO, format='%(name)s - %(levelname)s - %(message)s')
logger = logging.getLogger("test_full_extraction")

async def run_full_extraction():
    pdf_path = "data/UG_WHOLE_Spring_26_08_12_2025.pdf"
    if not os.path.exists(pdf_path):
        logger.error(f"File not found: {pdf_path}")
        return

    print(f"\n[{datetime.now().time()}] === 1. PARSING PDF ===")
    with open(pdf_path, "rb") as f:
        pdf_bytes = f.read()
    
    parsed_doc = await document_parser_service.parse_pdf(pdf_bytes)
    print(f"Parsed {parsed_doc.total_pages} pages.")

    print(f"\n[{datetime.now().time()}] === 2. CHUNKING ===")
    chunks = chunker_service.chunk_document(parsed_doc)
    print(f"Generated {len(chunks)} chunks.")

    print(f"\n[{datetime.now().time()}] === 3. LLM EXTRACTION (This may take a while...) ===")
    # Limit chunks for testing if needed, or pass all
    # For now, let's pass all but check if it's too huge. 
    # LLM Service logic currently chunks[:15] if no relevant tags found, but here we have Tags!
    
    uni_extraction = await extraction_service.extract_all(chunks)
    
    print("\n" + "="*50)
    print("🎓 EXTRACTION RESULTS")
    print("="*50)
    print(f"University: {uni_extraction.university_name}")
    print(f"Location: {uni_extraction.location}")
    print(f"\n📚 Departments ({len(uni_extraction.departments)}):")
    for d in uni_extraction.departments[:10]: # Show top 10
        print(f" - {d.name} ({len(d.programs)} programs)")
        for p in d.programs[:2]:
             print(f"   * Program: {p.name}")

    print(f"\n🏟️ Facilities ({len(uni_extraction.facilities)}):")
    for f in uni_extraction.facilities[:5]:
        print(f" - {f.name} ({f.facility_type})")

    print(f"\n💰 Fees ({len(uni_extraction.fee_structure)}):")
    for fee in uni_extraction.fee_structure[:10]:
         print(f" - {fee.fee_type}: {fee.amount} {fee.currency} (Program: {fee.program_name})")

    print(f"\n📅 Admissions:")
    if uni_extraction.admissions:
        criteria_preview = (uni_extraction.admissions.eligibility_criteria or "")[:100]
        print(f"Criteria: {criteria_preview}...")
        print(f"Deadlines: {uni_extraction.admissions.important_dates}")
    else:
        print("No admission info found.")

    # Save to JSON for inspection
    with open("scripts/test/extraction_result.json", "w") as f:
        f.write(uni_extraction.model_dump_json(indent=2))
    print(f"\nFull result saved to scripts/test/extraction_result.json")

if __name__ == "__main__":
    asyncio.run(run_full_extraction())
