# FULLSTACK-01-nextjs15-dotnet10-jobboard

## Problem
Build a recruiter-facing job board platform with a modern frontend and a .NET 10 backend API.

## Stack (planned)
- Backend: ASP.NET Core 10 Web API, EF Core 10, OpenAPI
- Frontend: Next.js 15 (TypeScript)
- Infra: Docker compose for local multi-service execution

## Current scaffold
- backend/src
- backend/tests
- frontend/web
- docs
- infra

## Milestone status
Vertical slice 1 complete: token-secured recruiter flow with SQL persistence and containerized local run.

Vertical slice 2 complete: backend integration tests for auth/jobs flow plus GitHub Actions CI workflow.

Vertical slice 3 complete: live deployment workflow added for Azure Container Apps (API) and Vercel (frontend).

## Next milestone
Capture runtime screenshots (UI and OpenAPI) from running services.