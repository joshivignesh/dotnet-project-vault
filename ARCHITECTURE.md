# Architecture

## Repository
dotnet-project-vault

## Purpose
This repository is a curated showcase of modern .NET engineering work by Vignesh Joshi. It is intended for recruiters, hiring managers, senior engineers, and companies evaluating backend and full-stack capability.

## Core goals
1. Showcase only current .NET stack choices.
2. Demonstrate enterprise-grade backend architecture and services.
3. Include selective full-stack projects with a .NET 10 backend.
4. Make the repository easy to scan, understand, and trust.

## Mandatory technology baseline
- .NET 10
- C# 14
- ASP.NET Core 10
- EF Core 10 where relevant
- OpenAPI / Swagger
- Docker where useful
- CI-ready layout
- health checks and production-minded defaults where relevant

## Non-goals
- Do not include legacy .NET projects.
- Do not add low-value toy demos.
- Do not create many unfinished folders without clear intent.

## Repository structure

Recommended top-level structure:

- 01-aspnetcore10-clean-architecture
- 02-minimal-api-jwt-openapi31
- 03-ef-core10-vector-search
- 04-signalr-realtime-dashboard
- 05-keda-worker-net10
- 06-aspire13-microservices
- 07-rate-limiting-middleware
- 08-sonarqube-github-actions
- FULLSTACK-01-nextjs15-dotnet10-jobboard
- FULLSTACK-02-blazor-saas-platform

## Priority order
1. Root README
2. FULLSTACK-01-nextjs15-dotnet10-jobboard
3. 01-aspnetcore10-clean-architecture scaffold
4. 02-minimal-api-jwt-openapi31 scaffold
5. 03-ef-core10-vector-search scaffold
6. Other project scaffolds
7. Additional polish and deployment notes

## Documentation model
Each project should communicate:
- what it does
- stack
- architecture
- local setup
- deployment idea
- implementation status

## Root README strategy
The root README must help a recruiter quickly answer:
- What can this developer build?
- Does the developer use modern .NET?
- Are there live demos or at least well-documented projects?
- Is there evidence of backend, cloud, and full-stack depth?

## Full-stack strategy
This repo should include .NET-backed full-stack work, but JS-first projects belong in js-fullstack-vault.
Cross-link between the two repos.

## Quality bar
- modern stack only
- practical project naming
- strong root README
- clean project summaries
- visible architectural maturity