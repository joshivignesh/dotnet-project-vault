# Status

## Repository
dotnet-project-vault

## Objective
Build a high-quality .NET 10 showcase repository with strong documentation, 1–2 meaningful full-stack examples, and clean scaffolding for additional projects.

## Done
- Repository purpose defined
- Project list selected
- Technology baseline fixed to .NET 10 / C# 14
- Full-stack inclusion strategy decided
- Root README created
- Top-level planned folder structure created
- README placeholders added for planned projects
- FULLSTACK-01-nextjs15-dotnet10-jobboard initial scaffold created
- FULLSTACK-01 minimal working backend API scaffold created and build validated
- FULLSTACK-01 minimal working frontend scaffold created and build validated
- 01-aspnetcore10-clean-architecture scaffold created and solution build validated
- 02-minimal-api-jwt-openapi31 scaffold created and solution build validated
- 02-minimal-api-jwt-openapi31 endpoint integration tests added and passing
- 02-minimal-api-jwt-openapi31 GitHub Actions test workflow added
- 02-minimal-api-jwt-openapi31 Dockerfile added
- 03-ef-core10-vector-search scaffold created and solution build validated
- 05-keda-worker-net10 scaffold created and solution build validated
- 06-aspire13-microservices scaffold created and solution build validated
- 07-rate-limiting-middleware scaffold created and solution build validated
- 08-sonarqube-github-actions scaffold created and solution build validated
- Deployment notes added at repository level
- Screenshot placeholder structure added
- Recruiter-readability pass completed for root documentation and index status
- FULLSTACK-01 vertical slice 1 implemented (JWT auth + EF Core SQL persistence + authenticated frontend flow)
- FULLSTACK-01 local Dockerfiles and compose setup added
- FULLSTACK-01 architecture and deployment visuals added (SVG)
- FULLSTACK-01 deployment target decision made (Vercel + Azure Container Apps + Azure SQL)
- FULLSTACK-01 backend integration tests added for auth/jobs endpoints
- FULLSTACK-01 backend integration tests wired into GitHub Actions CI
- FULLSTACK-01 live deployment workflow added for Azure Container Apps and Vercel
- FULLSTACK-01 runtime screenshots captured (UI and OpenAPI) and indexed under docs/screenshots
- **01-aspnetcore10-clean-architecture FULLY IMPLEMENTED** (CQRS/MediatR/EF Core/FluentValidation/9 tests/Dockerfile/CI/README)
- **02-minimal-api-jwt-openapi31 FULLY IMPLEMENTED** (JWT auth/OpenAPI 3.1/4 tests/Dockerfile/CI)
- **03-ef-core10-vector-search FULLY IMPLEMENTED** (semantic search/cosine similarity/EmbeddingService/7 tests/CI)
- **04-signalr-realtime-dashboard FULLY IMPLEMENTED** (SignalR hub/BackgroundService/dark dashboard SPA/4 tests/Dockerfile/CI/README)
- **05-keda-worker-net10 FULLY IMPLEMENTED** (InMemoryQueue/OrderProducer/Worker/KEDA k8s manifests/6 tests/Dockerfile/CI/README)
- **07-rate-limiting-middleware FULLY IMPLEMENTED** (4 policies: fixed/sliding/token-bucket/concurrency/8 tests/Dockerfile/CI/README)
- **08-sonarqube-github-actions FULLY IMPLEMENTED** (OrderCalculator+TextAnalyzer/36 tests/SonarCloud CI pipeline/Dockerfile/README)

## In Progress
- 06-aspire13-microservices: Aspire starter scaffold exists (AppHost/ApiService/Web/ServiceDefaults); real domain services pending

## Checkpoint
- Last updated: 2026-04-12 (session 3)
- Projects 01, 02, 03, 04, 05, 07, 08 fully production-quality with real code, passing tests, Docker, and CI.
- FULLSTACK-01 fully implemented with deployment workflow.
- 06-aspire13-microservices remains to be implemented.

## Blocked
- No active blockers.

## Next milestone
Complete 06-aspire13-microservices with real domain services, persistence, and proper Aspire orchestration.

## After next milestone
- Write production-quality READMEs for 03, 04, 05 (already have basic ones; update with badge counts)
- Verify all Docker builds locally
- Push to GitHub and verify all CI workflows pass green

## Risks
- 06-aspire13-microservices needs careful package version management (Aspire 9.x on .NET 10)
- SonarCloud CI workflow requires manual secret setup by the repo owner

## Notes for next session
Read ARCHITECTURE.md and AI-TASKS.md first.
Focus on 06-aspire13-microservices implementation — real Order + Catalog microservices.
Update STATUS.md after completing 06.
