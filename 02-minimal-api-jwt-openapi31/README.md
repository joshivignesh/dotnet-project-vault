# 02-minimal-api-jwt-openapi31

## Purpose
Planned sample for minimal APIs with JWT authentication and modern OpenAPI documentation.

## Stack
- .NET 10
- C# 14
- ASP.NET Core 10 Minimal APIs
- OpenAPI

## Status
Scaffolded

## Next step
Expand token issuing to real identity store and add endpoint tests.

## Current scaffold
- MinimalApiJwtOpenApi.slnx
- src/MinimalApiJwtOpenApi.Api
- tests/MinimalApiJwtOpenApi.Tests

## Current endpoints
- GET /health
- POST /api/auth/token
- GET /api/public
- GET /api/secure (requires bearer token)