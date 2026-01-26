"""
Quick test script to verify LLM connection and output quality.
Run this BEFORE the full extraction to ensure everything works.
"""
import sys
import os
import asyncio

sys.path.insert(0, os.path.abspath(os.path.join(os.path.dirname(__file__), '../../')))

from openai import AsyncOpenAI
from src.config import settings

async def test_basic_connection():
    """Test 1: Basic API connectivity"""
    print("=" * 50)
    print("TEST 1: Basic Connection")
    print("=" * 50)
    print(f"URL: {settings.llm_base_url}")
    print(f"Model: {settings.llm_model_name}")
    
    client = AsyncOpenAI(
        base_url=settings.llm_base_url,
        api_key="ollama",
        timeout=120,
    )
    
    try:
        # List models
        models = await client.models.list()
        print(f"\nAvailable models:")
        for m in models.data:
            print(f"  - {m.id}")
        print("\n✓ Connection successful!")
        return True
    except Exception as e:
        print(f"\n✗ Connection failed: {type(e).__name__}: {e}")
        return False


async def test_simple_completion():
    """Test 2: Simple text completion"""
    print("\n" + "=" * 50)
    print("TEST 2: Simple Completion")
    print("=" * 50)
    
    client = AsyncOpenAI(
        base_url=settings.llm_base_url,
        api_key="ollama",
        timeout=120,
    )
    
    try:
        response = await client.chat.completions.create(
            model=settings.llm_model_name,
            messages=[
                {"role": "user", "content": "What is 2+2? Reply with just the number."}
            ],
            max_tokens=10,
        )
        answer = response.choices[0].message.content
        print(f"Question: What is 2+2?")
        print(f"Answer: {answer}")
        print("\n✓ Completion works!")
        return True
    except Exception as e:
        print(f"\n✗ Completion failed: {type(e).__name__}: {e}")
        return False


async def test_json_extraction():
    """Test 3: JSON structured extraction (what we need for prospectus)"""
    print("\n" + "=" * 50)
    print("TEST 3: JSON Extraction (Critical for our use case)")
    print("=" * 50)
    
    client = AsyncOpenAI(
        base_url=settings.llm_base_url,
        api_key="ollama",
        timeout=180,
    )
    
    sample_text = """
    The University of Technology offers the following programs:
    
    Department of Computer Science:
    - Bachelor of Computer Science (4 years, $5000/year)
    - Master of Data Science (2 years, $7000/year)
    
    Department of Engineering:
    - Bachelor of Mechanical Engineering (4 years, $5500/year)
    """
    
    try:
        response = await client.chat.completions.create(
            model=settings.llm_model_name,
            messages=[
                {
                    "role": "system", 
                    "content": "You are a data extraction assistant. Always respond with valid JSON only."
                },
                {
                    "role": "user", 
                    "content": f"""Extract departments and programs from this text. 
Return JSON in this exact format:
{{"departments": [{{"name": "dept name", "programs": [{{"name": "program name", "duration": "X years", "fee": "$X"}}]}}]}}

TEXT:
{sample_text}"""
                }
            ],
            response_format={"type": "json_object"},
        )
        
        answer = response.choices[0].message.content
        print(f"Input: University prospectus sample")
        print(f"\nRaw LLM Output:")
        print("-" * 40)
        print(answer)
        print("-" * 40)
        
        # Try to parse JSON
        import json
        try:
            parsed = json.loads(answer)
            print(f"\n✓ Valid JSON! Parsed structure:")
            print(f"  Departments found: {len(parsed.get('departments', []))}")
            for dept in parsed.get('departments', []):
                print(f"    - {dept.get('name')}: {len(dept.get('programs', []))} programs")
            return True
        except json.JSONDecodeError as e:
            print(f"\n✗ Invalid JSON: {e}")
            return False
            
    except Exception as e:
        print(f"\n✗ Extraction failed: {type(e).__name__}: {e}")
        return False


async def test_with_real_chunk():
    """Test 4: Test with a real chunk from your PDF"""
    print("\n" + "=" * 50)
    print("TEST 4: Real PDF Chunk (Optional)")
    print("=" * 50)
    
    pdf_path = "data/UG_WHOLE_Spring_26_08_12_2025.pdf"
    if not os.path.exists(pdf_path):
        print(f"Skipping - PDF not found at {pdf_path}")
        return None
    
    from src.services.document_parser import document_parser_service
    from src.services.chunker import chunker_service
    
    print("Loading first few chunks from your PDF...")
    with open(pdf_path, "rb") as f:
        pdf_bytes = f.read()
    
    parsed_doc = await document_parser_service.parse_pdf(pdf_bytes)
    chunks = chunker_service.chunk_document(parsed_doc)
    
    # Get first chunk
    first_chunk = chunks[0]
    print(f"\nFirst chunk (page {first_chunk.page_number}, label: {first_chunk.section_label}):")
    print("-" * 40)
    print(first_chunk.text[:500] + "..." if len(first_chunk.text) > 500 else first_chunk.text)
    print("-" * 40)
    
    client = AsyncOpenAI(
        base_url=settings.llm_base_url,
        api_key="ollama",
        timeout=180,
    )
    
    try:
        response = await client.chat.completions.create(
            model=settings.llm_model_name,
            messages=[
                {
                    "role": "system", 
                    "content": "Extract the university name and any department/program names from this text. Return JSON."
                },
                {
                    "role": "user", 
                    "content": f"Extract info from:\n\n{first_chunk.text}"
                }
            ],
            response_format={"type": "json_object"},
        )
        
        print(f"\nLLM Response:")
        print(response.choices[0].message.content)
        print("\n✓ Real chunk processed!")
        return True
        
    except Exception as e:
        print(f"\n✗ Real chunk failed: {type(e).__name__}: {e}")
        return False


async def main():
    print("\n🔍 LLM CONNECTION & OUTPUT TEST")
    print("================================\n")
    
    results = {}
    
    # Run tests
    results['connection'] = await test_basic_connection()
    
    if results['connection']:
        results['completion'] = await test_simple_completion()
        results['json'] = await test_json_extraction()
        results['real_chunk'] = await test_with_real_chunk()
    
    # Summary
    print("\n" + "=" * 50)
    print("SUMMARY")
    print("=" * 50)
    for test, passed in results.items():
        status = "✓ PASS" if passed else ("⊘ SKIP" if passed is None else "✗ FAIL")
        print(f"  {test}: {status}")
    
    if all(v for v in results.values() if v is not None):
        print("\n🎉 All tests passed! Your LLM is ready for extraction.")
    else:
        print("\n⚠️  Some tests failed. Fix the issues before running full extraction.")


if __name__ == "__main__":
    asyncio.run(main())
