# 03 — Semantic Search with EF Core 10 Vectors

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)
![Tests](https://img.shields.io/badge/tests-7%20passing-brightgreen)

## What this is

A semantic search engine that understands meaning, not just keywords. Type "how do I install packages?" and it'll find docs about NuGet and dependency management — even if you didn't use those exact words.

No external AI API. Just EF Core, simple embeddings, and cosine similarity.

## How it works

1. **EmbeddingService** — converts text to numerical vectors (32-dimensional)
2. **Database stores vectors** — EF Core saves embeddings as JSON arrays
3. **Query** — convert user text to vector, find most similar docs using cosine distance
4. **Rank & return** — top matching documents sorted by relevance

Example:
```
User: "installing dependencies"
  ↓ (embed to vector)
Stored vectors for:
  - "NuGet package manager" (similarity: 0.92) ← rank 1
  - "npm install in Node" (similarity: 0.78) ← rank 2
  - "random tech article" (similarity: 0.34) ← rank 3
```

## Try it

```bash
cd 03-ef-core10-vector-search
dotnet run --project src/EfCoreVectorSearch.Api
```

**Add a document:**
```bash
curl -X POST http://localhost:5000/api/documents \
  -H "Content-Type: application/json" \
  -d '{"title":"Virtual Environments in Python","category":"python"}'
  # Auto-embedded and stored
```

**Search:**
```bash
curl "http://localhost:5000/api/search?q=python+virtual+env"
  # Returns most relevant documents with similarity scores
```

**List documents:**
```bash
curl "http://localhost:5000/api/documents?category=python"
```

## The embedding algorithm

**Simple but effective:**
- 32-dimensional vocabulary-based embeddings
- Extracts keywords from text
- Counts word frequency in a small vocabulary
- Normalizes to unit vectors (for cosine math)

This isn't GPT. It's intentionally lightweight so you can run it anywhere — no GPU needed, no API calls.

## Stack

| Component | Choice |
|-----------|--------|
| ORM | EF Core 10 with InMemory |
| Vectors | JSON arrays (`float[]`) stored in DB |
| Math | Manual cosine similarity calculation |
| Query | LINQ-to-Objects for ranking |
| Tests | xUnit with deterministic test data |

## Tests

7 tests covering:

```bash
dotnet test
```

- Vector generation consistency
- Cosine similarity calculation
- Normalization (vectors sum to 1.0)
- Relatedness ranking (similar docs score higher)
- Search returns top-K results

## Real-world uses

- **Product search** — find items by description, not just tags
- **Documentation** — search knowledge base by meaning
- **Content recommendations** — "users who read X also like Y"
- **Customer support** — match questions to similar resolved tickets

## Next steps

- Use SQL Server provider instead of InMemory
- Store vectors in SQL Server's native `vector(32)` type (SQL 2025+)
- Add batch embedding for bulk imports
- Integrate with Azure OpenAI embeddings if you need better quality

## Notes

The embedding quality is naive but deterministic. For production search over large text, consider Azure OpenAI or local models like `sentence-transformers`. This project shows the architecture — swap the embedding backend anytime.