# FULLSTACK-01 Architecture Notes

## Initial direction
- Monorepo-style folder layout with isolated backend and frontend.
- Backend-first contract design with OpenAPI.
- Frontend consumes backend API using typed clients.

## Deployment intent
- Containerized local dev using compose.
- Cloud target selected: Vercel (web), Azure Container Apps (API), Azure SQL Database.

## Current architecture

```mermaid
flowchart LR
	User[Recruiter Browser] --> Web[Next.js 15 Web]
	Web -->|JWT Login| Api[ASP.NET Core 10 API]
	Api -->|EF Core 10| Db[(SQL Server)]
	CI[GitHub Actions] --> Api
	CI --> Web
```

## Visual assets
- architecture-overview.svg
- deployment-topology.svg