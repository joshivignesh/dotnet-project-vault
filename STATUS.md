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

## In progress
- Capture runtime screenshots (UI and OpenAPI) from running services

## Checkpoint
- Last updated: 2026-04-11
- Repository is in a stable pause state after FULLSTACK-01 live deployment workflow milestone.
- Safe resume point: capture runtime screenshots and update docs.

## Blocked
- Need runtime screenshots (UI and OpenAPI) captured from running services

## Next milestone
Capture runtime screenshots (UI and OpenAPI) from running services.

## After next milestone
Add environment configuration strategy for development/staging/production.

## Risks
- Too many scaffolds without enough execution depth
- README quality falling behind code generation
- Mixing JS-heavy projects that belong in a separate repo

## Notes for next session
Read ARCHITECTURE.md and AI-TASKS.md first.
Focus on runtime screenshot capture and documentation updates next.
Then define environment strategy for development/staging/production.
Update this file after each milestone.