# 01 — Clean Architecture with CQRS + MediatR

> **Product Catalog API** — properly layered, fully tested, production-ready architecture.

[![CI](https://github.com/vignesh-joshi-dev/dotnet-project-vault/actions/workflows/01-clean-architecture-ci.yml/badge.svg)](https://github.com/vignesh-joshi-dev/dotnet-project-vault/actions/workflows/01-clean-architecture-ci.yml)

---

## What you'll see

- **Clean Architecture** — Domain, Application, Infrastructure, API layers properly separated
- **CQRS** via [MediatR](https://github.com/jbogard/MediatR) — reads and writes go through command/query handlers
- **EF Core 10** — InMemory by default, swap to SQL Server with a connection string
- **FluentValidation** — all commands validated before execution
- **Minimal API** — route groups in ASP.NET Core 10
- **OpenAPI docs** — auto-generated, interactive API docs
- **Tests** — 9 test scenarios covering all handlers, CRUD operations, and edge cases

---

## Architecture

```
CleanArchitecture.Domain          → Entities, interfaces (no external dependencies)
CleanArchitecture.Application     → MediatR commands/queries, validators, DTOs
CleanArchitecture.Infrastructure  → EF Core, repositories, DI wiring
CleanArchitecture.Api             → Minimal API endpoints, startup, OpenAPI
CleanArchitecture.UnitTests       → Handler tests using InMemory DB
```

### Layer dependency direction
```
API → Application → Domain
         ↑
  Infrastructure (implements domain interfaces)
```

---

## Stack

| Layer | Technology |
|-------|-----------|
| Framework | ASP.NET Core 10 Minimal API |
| CQRS | MediatR 12 |
| Validation | FluentValidation 11 |
| ORM | EF Core 10 (InMemory / SQL Server) |
| Testing | xUnit, NSubstitute |
| Container | Docker |
| CI | GitHub Actions |

---

## API Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/health` | Health check |
| GET | `/api/products` | List all products (optional `?category=` filter) |
| GET | `/api/products/{id}` | Get product by ID |
| POST | `/api/products` | Create a new product |
| PUT | `/api/products/{id}` | Update a product |
| DELETE | `/api/products/{id}` | Delete a product |

OpenAPI docs available at `/openapi/v1.json` in development.

---

## Quick Start

### Option 1 — dotnet run

```bash
cd 01-aspnetcore10-clean-architecture
dotnet run --project src/CleanArchitecture.Api
# API running at http://localhost:5000
# OpenAPI at http://localhost:5000/openapi/v1.json
```

### Option 2 — Docker

```bash
docker build -t clean-arch-api .
docker run -p 8080:8080 clean-arch-api
# API at http://localhost:8080
```

### Option 3 — With SQL Server

Set a connection string to switch from InMemory to SQL Server:

```bash
dotnet run --project src/CleanArchitecture.Api \
  --ConnectionStrings__DefaultConnection="Server=localhost;Database=CleanArchDB;Trusted_Connection=True;"
```

---

## Running Tests

```bash
dotnet test
# 9/9 tests passing
```

Tests cover: Create, Read all, Read by ID, Read filtered by category, Update, Delete (existing), Delete (not found) handlers.

---

## Deployment

### Try it live (coming soon)
Once deployed, this API will be live at: `https://clean-arch-api.railway.app`

To deploy this yourself:

**Option 1: Railway (most straightforward)**
1. Go to [railway.app](https://railway.app), sign in with GitHub
2. Create new project → Deploy from GitHub
3. Select this repo, folder: `01-aspnetcore10-clean-architecture`
4. Railway reads the Dockerfile and deploys automatically
5. Get a live URL within 60 seconds

**Option 2: Docker locally**
```bash
docker build -t clean-arch:latest .
docker run -p 8080:8080 clean-arch:latest
# API at http://localhost:8080/openapi/v1.json
```

**Option 3: Azure Container Apps**
```bash
az containerapp create \
  --name clean-arch-api \
  --resource-group my-rg \
  --image myregistry.azurecr.io/clean-arch:latest \
  --target-port 8080 \
  --ingress external
```

---

## Architecture Notes

The domain layer has **zero external dependencies** — only entities and interface contracts. This makes the core business logic completely portable and testable.

`Product.Create()` is a factory method that enforces domain invariants (no negative prices, non-null required fields) at construction time, preventing invalid state across the application.

The CQRS separation via MediatR means reads and writes can be scaled and optimized independently — a pattern that scales cleanly toward event sourcing in more complex systems.

## Status
**COMPLETE** — Full implementation with 9/9 tests passing, Dockerfile, and CI workflow.

## Next step
Fork this, deploy to Railway, and modify for your own domain (Authors & Books, Orders & Items, etc).

## Current scaffold
- CleanArchitecture.slnx
- src/CleanArchitecture.Api
- src/CleanArchitecture.Application
- src/CleanArchitecture.Domain
- src/CleanArchitecture.Infrastructure
- tests/CleanArchitecture.UnitTests 