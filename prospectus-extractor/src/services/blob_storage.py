import logging
import asyncio
from typing import Optional, List
from azure.storage.blob.aio import BlobServiceClient, BlobClient, ContainerClient
from azure.core.exceptions import ResourceExistsError, ResourceNotFoundError
from azure.storage.blob import ContentSettings
import certifi
from src.config import settings

logger = logging.getLogger(__name__)

class BlobStorageService:
    def __init__(self):
        self.connection_string = settings.azure_storage_connection_string
        self.container_name = settings.azure_storage_container
        # Explicitly use certifi's CA bundle for SSL verification to fix macOS issues
        self._blob_service_client = BlobServiceClient.from_connection_string(
            self.connection_string,
            connection_verify=certifi.where()
        )

    async def close(self):
        """Close the client session explicitly"""
        await self._blob_service_client.close()

    async def _get_container_client(self) -> ContainerClient:
        """ Get the container client using the persistent service client"""
        return self._blob_service_client.get_container_client(self.container_name)

    async def upload(self, blob_name:str, data:bytes, content_type:str = "application/pdf", metadata:Optional[dict] = None) -> str:
        try:
            container_client = await self._get_container_client()
            blob_client = container_client.get_blob_client(blob_name)
            content_settings = ContentSettings(content_type = content_type)
            logger.info(f"Uploading blob: {blob_name} ({len(data)}bytes)")
            #Native async upload
            await blob_client.upload_blob(data, overwrite=True, content_settings=content_settings, metadata=metadata)
            return blob_client.url
        except Exception as e:
            logger.error(f"Error uploading blob:{blob_name}: {str(e)}")
            raise

    async def download(self, blob_name:str) -> bytes:
        try:
            container_client = await self._get_container_client()
            blob_client = container_client.get_blob_client(blob_name)
            logger.info(f"Downloading blob: {blob_name}")
            #Native async download
            stream = await blob_client.download_blob()
            return await stream.readall()
        except ResourceNotFoundError:
            logger.error(f"Blob Not found: {blob_name}")
            raise
        except Exception as e:
            logger.error(f"Error downloading blob {blob_name}: {str(e)}")
            raise

    async def delete (self, blob_name:str) ->None:
        try:
            container_client = await self._get_container_client()
            blob_client = container_client.get_blob_client(blob_name)
            await blob_client.delete_blob()
            logger.info(f"Deleted blob:{blob_name}")
        except ResourceNotFoundError:
            logger.warning(f"Attempted to delete non-existent blob:{blob_name}")
        except Exception as e:
            logger.error(f"Error deleting blob {blob_name} : {str(e)}")
            raise

    async def exists(self, blob_name:str)->bool:
        container_client = await self._get_container_client()
        blob_client = container_client.get_blob_client(blob_name)
        return await blob_client.exists()

    async def list_blobs(self, prefix:Optional[str]=None)-> List[str]:
        container_client = await self._get_container_client()
        blob_list =[]
        # Native async iteration
        async for blob in container_client.list_blobs(name_starts_with=prefix):
            blob_list.append(blob.name)
        return blob_list

#Singleton instance
blob_storage = BlobStorageService()


