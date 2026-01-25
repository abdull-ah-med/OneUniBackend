from pydantic_settings import BaseSettings
from pydantic import Field


class Settings(BaseSettings):
    # Database
    database_url: str = Field(..., description="PostgreSQL connection string")

    # Azure Storage
    azure_storage_connection_string: str = Field(..., description="Azure Storage connection")
    azure_storage_container: str = Field(default="prospectuses")

    # Azure Service Bus
    azure_servicebus_connection_string: str = Field(..., description="Service Bus connection")
    azure_servicebus_queue_name: str = Field(default="prospectus-extraction-jobs")

    # LLM Service (Ollama)
    llm_base_url: str = Field(default="http://localhost:11434/v1")
    llm_model_name: str = Field(default="llama3.1:8b")
    groq_api_key: str = Field(default="", description="Groq API Key")
    llm_temperature: float = Field(default=0.1)
    llm_max_tokens: int = Field(default=4096)

    # Embedding Service (Ollama)
    embedding_base_url: str = Field(default="http://localhost:11434")
    embedding_model_name: str = Field(default="mxbai-embed-large")
    embedding_dimensions: int = Field(default=1024)

    # Processing
    chunk_size: int = Field(default=1000)
    chunk_overlap: int = Field(default=200)

    class Config:
        env_file = ".env"
        env_file_encoding = "utf-8"
        extra = "ignore"


settings = Settings()