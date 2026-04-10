# 03-ef-core10-vector-search

## Purpose
Planned data-focused sample demonstrating EF Core 10 and vector-search-adjacent query patterns.

## Stack
- .NET 10
- C# 14
- EF Core 10
- SQL database provider (TBD)

## Status
Scaffolded

## Next step
Add migration strategy and replace sample vectors with provider-native vector indexing.

## Current scaffold
- EfCoreVectorSearch.slnx
- src/EfCoreVectorSearch.Api
- src/EfCoreVectorSearch.Domain
- src/EfCoreVectorSearch.Infrastructure
- tests/EfCoreVectorSearch.Tests

## Current endpoints
- GET /health
- GET /api/documents
- POST /api/search