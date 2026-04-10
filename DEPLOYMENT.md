# Deployment Notes

## Overview
This repository is organized for incremental deployment readiness. Current deployment posture is scaffold-first with production intent.

## Baseline targets
- Containerized API services with Docker
- Managed SQL service (Azure SQL or PostgreSQL based on project fit)
- Frontend hosting for full-stack project (Vercel or Azure Static Web Apps)
- CI pipeline validation via GitHub Actions

## Current state by project
- FULLSTACK-01-nextjs15-dotnet10-jobboard: local backend/frontend builds validated
- 01-aspnetcore10-clean-architecture: solution scaffold and compile validation complete
- 02-minimal-api-jwt-openapi31: JWT-ready minimal API scaffold and compile validation complete
- 03-ef-core10-vector-search: EF Core + SQL scaffold and compile validation complete
- 05-keda-worker-net10: worker scaffold and compile validation complete
- 06-aspire13-microservices: Aspire starter scaffold and compile validation complete
- 07-rate-limiting-middleware: middleware scaffold and compile validation complete
- 08-sonarqube-github-actions: CI + SonarQube scaffold and compile validation complete

## Next deployment milestones
1. Add Dockerfiles and compose for flagship FULLSTACK-01 backend/frontend
2. Add environment configuration strategy for development/staging/production
3. Add cloud deployment workflow for one backend project and one full-stack project
4. Enable SonarCloud quality gate on pull requests

## Required repository secrets (planned)
- SONAR_TOKEN
- SONAR_HOST_URL
- AZURE_CREDENTIALS (or cloud equivalent)
- Connection-string secrets per deployed service