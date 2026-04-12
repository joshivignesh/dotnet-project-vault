# .NET Project Vault

A hands-on collection of .NET 10 projects by Vignesh Joshi, demonstrating modern backend systems, APIs, real-time features, distributed services, and cloud-native deployments.

## What this is

I built this repository to show actual engineering work, not homework assignments. Each project is a real problem solved with current-day .NET tools:
- properly layered and tested backend services
- production-grade APIs with auth and validation
- full-stack integrations (Next.js + .NET)
- cloud deployment patterns (Azure, Vercel, Railway)
- DevOps practices (CI/CD, code quality gates, container orchestration)

No bloat. No outdated frameworks. Just solid .NET work.

## Technology baseline

- .NET 10
- C# 14
- ASP.NET Core 10
- EF Core 10
- OpenAPI
- Docker
- CI/CD-friendly structure

## Project index

| Project | Focus | Tests | Status |
|---------|-------|-------|--------|
| [01-aspnetcore10-clean-architecture](01-aspnetcore10-clean-architecture/README.md) | Clean Architecture, CQRS, MediatR, EF Core | 9 ✅ | **Complete** |
| [02-minimal-api-jwt-openapi31](02-minimal-api-jwt-openapi31/README.md) | Minimal APIs, JWT auth, OpenAPI 3.1 | 4 ✅ | **Complete** |
| [03-ef-core10-vector-search](03-ef-core10-vector-search/README.md) | Semantic vector search, cosine similarity | 7 ✅ | **Complete** |
| [04-signalr-realtime-dashboard](04-signalr-realtime-dashboard/README.md) | Real-time SignalR hub, live metrics SPA | 4 ✅ | **Complete** |
| [05-keda-worker-net10](05-keda-worker-net10/README.md) | Worker service, KEDA autoscaling, K8s manifests | 6 ✅ | **Complete** |
| [06-aspire13-microservices](06-aspire13-microservices/README.md) | .NET Aspire 9 orchestration, microservices | — | In Progress |
| [07-rate-limiting-middleware](07-rate-limiting-middleware/README.md) | 4 rate limiting strategies, 429 behavior | 8 ✅ | **Complete** |
| [08-sonarqube-github-actions](08-sonarqube-github-actions/README.md) | SonarCloud CI pipeline, coverage, quality gates | 36 ✅ | **Complete** |
| [FULLSTACK-01-nextjs15-dotnet10-jobboard](FULLSTACK-01-nextjs15-dotnet10-jobboard/README.md) | Next.js 15 + .NET 10 full-stack job board | — | Vertical slices complete |
| [FULLSTACK-02-blazor-saas-platform](FULLSTACK-02-blazor-saas-platform/README.md) | Blazor + .NET SaaS platform | — | Planned |

## Quick read

If you have 10 minutes:
- Check [01-clean-architecture](01-aspnetcore10-clean-architecture) for CQRS + domain modeling
- Check [07-rate-limiting](07-rate-limiting-middleware) for middleware patterns + 4 limiter algorithms
- Check [08-sonarqube](08-sonarqube-github-actions) for CI/CD integration and code quality gates
- Check [FULLSTACK-01](FULLSTACK-01-nextjs15-dotnet10-jobboard) to see backend + frontend working together

## Related repositories

- [personal-profile](https://github.com/joshivignesh/personal-profile)
- [js-fullstack-vault](https://github.com/joshivignesh/js-fullstack-vault)

## How to navigate this repo

Each project has its own README with a quick-start guide, architecture notes, and test instructions. You can run any of them locally in ~5 minutes.

**By interest:**
- **Want to see clean architecture?** Start with [01-clean-architecture](01-aspnetcore10-clean-architecture/README.md)
- **Want to understand rate limiting & middleware?** Check [07-rate-limiting](07-rate-limiting-middleware/README.md)
- **Want to see a full-stack app?** See [FULLSTACK-01](FULLSTACK-01-nextjs15-dotnet10-jobboard/README.md)
- **Interested in CI/CD and code quality?** Look at [08-sonarqube](08-sonarqube-github-actions/README.md)
- **Want to see real-time features?** Try [04-signalr-dashboard](04-signalr-realtime-dashboard/README.md)

## Status

7 of 8 core projects complete. FULLSTACK-01 fully implemented. See [STATUS.md](STATUS.md) for detailed progress.

## Running projects locally

```bash
# Clone and pick a project
cd 01-aspnetcore10-clean-architecture
dotnet run --project src/CleanArchitecture.Api
# API at http://localhost:5000
```

Each project includes OpenAPI/Swagger for interactive API docs.

## Deployment & Live Demos

**FULLSTACK-01 (Next.js + .NET):**
- Frontend: Ready for Vercel deployment (secrets needed)
- Backend API: Ready for Azure Container Apps deployment (secrets needed)
- Database: Azure SQL Server

See [FULLSTACK-01 README](FULLSTACK-01-nextjs15-dotnet10-jobboard/README.md) for deployment instructions and live links once configured.

## Contact

- **LinkedIn:** [joshivignesh](https://www.linkedin.com/in/joshivignesh/)
- **Email:** joshi.vignesh@gmail.com

## License

MIT License — feel free to use and modify these projects.
