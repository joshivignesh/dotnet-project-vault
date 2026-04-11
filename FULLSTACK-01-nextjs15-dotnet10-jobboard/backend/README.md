# Backend

## Goal
Host the .NET 10 API for job listings, employers, and candidate workflows.

## Planned components
- API project (ASP.NET Core 10)
- Application/domain layers as needed
- Persistence with EF Core 10
- Contract-first OpenAPI coverage

## Status
Vertical slice implemented with EF Core SQL persistence and JWT authentication.

## Current endpoints
- GET /health
- GET /api/public
- POST /api/auth/token
- GET /api/jobs (requires bearer token)
- GET /api/jobs/{id} (requires bearer token)
- POST /api/jobs (requires bearer token)

## Auth defaults
- Demo credentials for local use: demo / demo
- JWT issuer and audience configured in appsettings

## Run locally
1. Change to backend project folder:
	- FULLSTACK-01-nextjs15-dotnet10-jobboard/backend/src/JobBoard.Api
2. Run:
	- dotnet run

## Run in Docker
1. Change to infra folder:
	- FULLSTACK-01-nextjs15-dotnet10-jobboard/infra
2. Start compose:
	- docker compose up --build

## Automated tests
- Integration tests project:
	- FULLSTACK-01-nextjs15-dotnet10-jobboard/backend/tests/JobBoard.Api.IntegrationTests
- Run locally:
	- dotnet test FULLSTACK-01-nextjs15-dotnet10-jobboard/backend/tests/JobBoard.Api.IntegrationTests/JobBoard.Api.IntegrationTests.csproj
- CI workflow:
	- .github/workflows/fullstack01-backend-tests.yml