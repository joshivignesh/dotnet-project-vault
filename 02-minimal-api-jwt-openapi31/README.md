# 02-minimal-api-jwt-openapi31

## Purpose
Planned sample for minimal APIs with JWT authentication and modern OpenAPI documentation.

## Stack
- .NET 10
- C# 14
- ASP.NET Core 10 Minimal APIs
- OpenAPI

## Status
Scaffold complete with endpoint integration tests and project-scoped CI workflow.

## Next step
Expand token issuing to a real identity store and role model.

## Current scaffold
- MinimalApiJwtOpenApi.slnx
- src/MinimalApiJwtOpenApi.Api
- tests/MinimalApiJwtOpenApi.Tests

## Current endpoints
- GET /health
- POST /api/auth/token
- GET /api/public
- GET /api/secure (requires bearer token)

## Test coverage
- GET /health returns 200
- POST /api/auth/token returns bearer token for demo credentials
- GET /api/secure returns 401 without token
- GET /api/secure returns 200 with valid token

## CI workflow
- .github/workflows/minimal-api-jwt-openapi31-tests.yml