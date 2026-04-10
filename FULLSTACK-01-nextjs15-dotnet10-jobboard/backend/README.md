# Backend

## Goal
Host the .NET 10 API for job listings, employers, and candidate workflows.

## Planned components
- API project (ASP.NET Core 10)
- Application/domain layers as needed
- Persistence with EF Core 10
- Contract-first OpenAPI coverage

## Status
Minimal working API scaffold complete

## Current endpoints
- GET /health
- GET /api/jobs
- GET /api/jobs/{id}
- POST /api/jobs

## Run locally
1. Change to backend project folder:
	- FULLSTACK-01-nextjs15-dotnet10-jobboard/backend/src/JobBoard.Api
2. Run:
	- dotnet run