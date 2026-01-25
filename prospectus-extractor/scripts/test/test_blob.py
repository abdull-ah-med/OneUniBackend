import asyncio
import sys
sys.path.insert(0, ".")

from src.services.blob_storage import blob_storage
import logging
import src.services.blob_storage

logging.basicConfig(level=logging.INFO)
async def test_blob_service():

    test_data = b"Hello, this is a test PDF Content"
    blob_name = "test/test_document.txt"

    url = await blob_storage.upload(blob_name, test_data, content_type="text/plain")
    print(f"Uploaded to: {url}")

    #Test exists
    exists = await blob_storage.exists(blob_name)
    print(f"Exists: {exists}")

    #Test download
    downloaded_data = await blob_storage.exists(blob_name)
    print(f"Downloaded data: {downloaded_data}")

    #Test list
    blobs = await blob_storage.list_blobs(prefix="test/")
    print(f"Blobs: {blobs}")

    # Test delete
    await blob_storage.delete(blob_name)
    print("deleted")

if __name__ == "__main__":
    asyncio.run(test_blob_service())
