import asyncio
import sys
import os
import logging
import uuid
from datetime import datetime

# Add src to python path to allow imports
sys.path.insert(0, os.path.abspath(os.path.join(os.path.dirname(__file__), '../src')))

from services.document_parser import document_parser_service
from services.blob_storage import blob_storage

# Configure logging
logging.basicConfig(level=logging.INFO, format='%(name)s - %(levelname)s - %(message)s')
logger = logging.getLogger("test_script")

async def test_full_pipeline():
    pdf_path = "data/UG_WHOLE_Spring_26_08_12_2025.pdf"
    
    if not os.path.exists(pdf_path):
        logger.error(f"File not found: {pdf_path}")
        return

    print(f"\n[{datetime.now().time()}] === 1. READING LOCAL PDF ===")
    logger.info(f"Reading PDF from {pdf_path}...")
    with open(pdf_path, "rb") as f:
        pdf_bytes = f.read()
    print(f"Read {len(pdf_bytes)} bytes.")

    print(f"\n[{datetime.now().time()}] === 2. UPLOADING TO BLOB STORAGE ===")
    blob_name = f"test_uploads/{uuid.uuid4()}_prospectus.pdf"
    try:
        blob_url = await blob_storage.upload(blob_name, pdf_bytes, content_type="application/pdf")
        print(f"✅ Upload successful!")
        print(f"Blob URL: {blob_url}")
        print(f"Blob Name: {blob_name}")
    except Exception as e:
        logger.error(f"Upload failed: {e}")
        return

    print(f"\n[{datetime.now().time()}] === 3. DOWNLOADING FROM BLOB STORAGE ===")
    try:
        downloaded_bytes = await blob_storage.download(blob_name)
        print(f"✅ Download successful!")
        print(f"Downloaded Size: {len(downloaded_bytes)} bytes")
        
        if len(downloaded_bytes) != len(pdf_bytes):
            logger.error("❌ Size mismatch between uploaded and downloaded bytes!")
            return
        else:
            print("✅ Byte size matches exactly.")
    except Exception as e:
        logger.error(f"Download failed: {e}")
        return

    print(f"\n[{datetime.now().time()}] === 4. PARSING DOCUMENT ===")
    try:
        # Define start time
        start_time = datetime.now()
        
        parsed_doc = await document_parser_service.parse_pdf(downloaded_bytes)
        
        # Calculate duration
        duration = datetime.now() - start_time
        
        print(f"✅ Parsing successful in {duration.total_seconds():.2f} seconds!")
        
        print("\n" + "="*60)
        print(f"📄 PARSING REPORT")
        print("="*60)
        print(f"Total Pages: {parsed_doc.total_pages}")
        print(f"Metadata:   {parsed_doc.metadata}")
        
        # Stats
        total_chars = sum(len(p.text) for p in parsed_doc.pages)
        total_tables = sum(len(p.tables) for p in parsed_doc.pages)
        print(f"Total Text Characters: {total_chars}")
        print(f"Total Tables Detected: {total_tables}")

        print("\n" + "-"*30)
        print("🔍 SAMPLE CONTENT (Page 1)")
        print("-" * 30)
        if parsed_doc.pages:
            p1 = parsed_doc.pages[0]
            print(p1.text[:1000]) # First 1000 chars
            
            if p1.tables:
                print(f"\nFound {len(p1.tables)} tables on page 1. First table preview:")
                # Simple table print
                for row in p1.tables[0][:3]: # First 3 rows
                    print(row)
        print("="*60)

    except Exception as e:
        logger.error(f"Parsing failed: {e}")
        raise
    
    finally:
        # Cleanup
        print(f"\n[{datetime.now().time()}] === 5. CLEANUP ===")
        # await blob_storage.delete(blob_name)
        # print("Test blob deleted.")
        await blob_storage.close()

if __name__ == "__main__":
    asyncio.run(test_full_pipeline())
