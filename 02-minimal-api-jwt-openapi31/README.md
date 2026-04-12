# 02 — Minimal API with JWT Auth & OpenAPI 3.1

![CI](https://github.com/vignesh-joshi-dotnet/dotnet-project-vault/actions/workflows/minimal-api-jwt-openapi31-tests.yml/badge.svg)
![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)
![Tests](https://img.shields.io/badge/tests-4%20passing-brightgreen)

## What this is

A minimal API that shows JWT bearer token authentication in action. Get a token, use it to access protected endpoints, see how 401/403 errors work.

Minimal APIs are ASP.NET Core without the bloat. 50 lines of code replaces what used to take 100+.

## Try it

```bash
cd 02-minimal-api-jwt-openapi31
dotnet run --project src/MinimalApiJwtOpenApi.Api
```

The API comes with OpenAPI docs at: `http://localhost:5000/openapi/v1.json`

### Get a token

```bash
curl -X POST http://localhost:5000/token \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"password"}'
```

Returns:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "expiresIn": 3600
}
```

### Use the token

```bash
curl http://localhost:5000/secure \
  -H "Authorization: Bearer eyJhbGc..."
```

Returns: `{ "message": "This is secure" }`

### Without token

```bash
curl http://localhost:5000/secure
# Returns 401 Unauthorized
```

## Stack

| Component | Choice |
|-----------|--------|
| API style | ASP.NET Core 10 Minimal API |
| Auth | JWT (HS256) |
| OpenAPI | Built-in (v3.1) |
| Tests | xUnit + WebApplicationFactory |

## Tests

4 integration tests:

```bash
dotnet test
```

- Token generation works
- Valid token grants access
- Missing token returns 401
- Invalid token returns 401

## How the auth works

1. **POST /token** → validate credentials, issue JWT
2. **JWT payload** includes username, issued-at, expires-at
3. **All requests** check for `Authorization: Bearer <token>` header
4. **Token valid?** Process request
5. **Token expired/missing/invalid?** Return 401

The JWT secret is hardcoded for demo (obviously, use Azure Key Vault or environment variables in production).

## Deployment

Docker:
```bash
docker build -t jwt-api .
docker run -p 8080:8080 jwt-api
```

Railway:
```bash
# Push to GitHub, railway.app auto-detects Dockerfile
```

## Notes

- Tokens expire in 1 hour (configurable in `Program.cs`)
- This demo uses a static username/password (admin/password)
- Real apps: validate against a user database or OAuth provider
- Add role-based access control (RBAC) by adding "role" claim to token
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