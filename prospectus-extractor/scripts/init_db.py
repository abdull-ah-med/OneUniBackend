"""
Run this script once to create the prospectus tables and enable pgvector.
Usage: python -m scripts.init_db
"""
import sys
sys.path.insert(0, "src")
from sqlalchemy import text
from config import settings
from models.db import Base, get_engine

def init_database():
    engine = get_engine(settings.database_url)

    with engine.connect() as conn:
        conn.execute(text("CREATE EXTENSION IF NOT EXISTS vector"))
        conn.commit()
        print("pgvector extension enables")
    Base.metadata.create_all(engine)
    print("\n\n Database initialization complete\n")

if __name__ == "__main__":
    init_database()