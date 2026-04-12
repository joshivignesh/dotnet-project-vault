# FULLSTACK-01 — Next.js 15 + .NET 10 Job Board

A working full-stack SaaS demo: recruiter-focused job board with Next.js 15 frontend and .NET 10 backend API.

## What's in the box

- **Backend** — ASP.NET Core 10 Web API with JWT auth, EF Core 10 SQL, OpenAPI docs
- **Frontend** — Next.js 15 (TypeScript) dashboard for job posting and applicant review
- **Database** — SQL Server for job listings, applicants, recruiter accounts
- **Local run** — Docker Compose for full-stack development
- **Cloud ready** — Deployment workflows for Vercel (frontend) + Azure Container Apps (API)

## Quick start

### Local development (Docker Compose)
```bash
cd FULLSTACK-01-nextjs15-dotnet10-jobboard

# Spin up all services locally
docker-compose up

# Frontend at http://localhost:3000
# API at http://localhost:5000
# OpenAPI docs at http://localhost:5000/openapi/v1.json
```

### Manual setup
```bash
# Terminal 1: Backend
cd backend
dotnet run --project src/JobBoard.Api

# Terminal 2: Frontend
cd frontend/web
npm install
npm run dev
```

## How it works

1. **Recruiter login** — Email/password auth via JWT token (stored in HTTP-only cookie)
2. **Post job** — Create listing with title, description, required skills
3. **Browse applicants** — See all candidates who applied, filter by status
4. **Hire workflow** — Accept/reject applicants, notes included

## Live demo

**Once deployed, these live links will be active:**
- Frontend: `https://jobboard-nextjs.vercel.app` (will be live after Vercel deployment)
- API: `https://jobboard-api.azurecontainerapps.io` (will be live after Azure deployment)

See [deployment setup](#deployment-setup) below to enable this.

## Deployment setup

### Prerequisites
- Vercel account (free tier OK)
- Azure account with Container Apps enabled
- Azure SQL Server or Azure Database for SQL
- GitHub repository (fork or clone this repo)

### Deploy the backend API

**Option A: Azure Container Apps (production-recommended)**

1. Create Azure Container Apps environment:
```bash
az containerapp create \
  --name jobboard-api \
  --resource-group my-rg \
  --environment my-env \
  --image jobboard-api:latest \
  --target-port 5000 \
  --ingress external
```

2. Add GitHub Actions secrets to your repo:
   - `AZURE_CLIENT_ID`
   - `AZURE_TENANT_ID`
   - `AZURE_SUBSCRIPTION_ID`
   - `AZURE_RESOURCE_GROUP`
   - `AZURE_CONTAINER_APP_NAME`
   - `FULLSTACK01_DB_CONNECTION_STRING` (Azure SQL connection string)
   - `FULLSTACK01_JWT_KEY` (random 32+ char string)

3. Push to `main` branch — deployment workflow runs automatically.

**Option B: Railway (faster setup)**

1. Go to [railway.app](https://railway.app)
2. Create new project → Deploy from GitHub
3. Select this repo and `FULLSTACK-01-nextjs15-dotnet10-jobboard` folder
4. Add `FULLSTACK01_DB_CONNECTION_STRING` and `FULLSTACK01_JWT_KEY` variables
5. Deploy button — done in seconds

### Deploy the frontend

**Vercel (recommended for Next.js)**

1. Go to [vercel.com](https://vercel.com) and sign in with GitHub
2. **Import project** → select this repo
3. Set root directory to `FULLSTACK-01-nextjs15-dotnet10-jobboard/frontend/web`
4. Add environment variable:
   - `NEXT_PUBLIC_API_BASE_URL` = your API URL (e.g., `https://jobboard-api.azurecontainerapps.io`)
5. Click **Deploy** — Vercel builds and hosts automatically

After ~2 min, you'll get a live URL like `https://jobboard-nextjs.vercel.app`.

## Architecture

```
┌─────────────────────────────────────────────────────┐
│                 Internet                             │
└────────────────┬────────────────────────────────────┘
                 │
        ┌────────┴────────┐
        │                 │
    ┌───▼───┐         ┌───▼────┐
    │Vercel │         │ Azure  │
    │(Next) │◄────────┤ Con    │
    └───────┘  HTTP   │ Apps   │
                      │(API)   │
                      └───┬────┘
                          │
                      ┌───▼────────┐
                      │ Azure SQL  │
                      │ Server     │
                      └────────────┘
```

## Test coverage

**Backend integration tests:**
- JWT token generation & validation
- Recruiter login flow
- Job posting creation
- Applicant application submission
- Database persistence via EF Core

Run locally:
```bash
cd backend
dotnet test tests/JobBoard.Api.IntegrationTests
```

**Frontend:**
- Page rendering
- Form submission
- Token refresh on page reload

## Files & structure

```
FULLSTACK-01-nextjs15-dotnet10-jobboard/
  backend/
    src/JobBoard.Api/           ← ASP.NET Core API
      Program.cs                ← JWT setup, CORS, EF Core config
      Controllers/             ← Auth, Jobs, Applicants endpoints
      Models/
      Data/
    tests/
      JobBoard.Api.IntegrationTests/
    Dockerfile                  ← Multi-stage build (SDK → runtime)
    docker-compose.yml         ← Local PostgreSQL + API
  frontend/
    web/                        ← Next.js app
      pages/                    ← Login, Dashboard, Jobs, Applicants
      app/                      ← App directory (Next.js 15)
      components/              ← Reusable UI
    Dockerfile
  docs/
    screenshots/               ← Runtime UI & API dashboard captures
  .github/workflows/
    fullstack01-deploy.yml     ← Automated deployment
```

## Common issues

**"Could not connect to database"**
→ Check `ConnectionString` in `appsettings.json` and `FULLSTACK01_DB_CONNECTION_STRING` env var

**"CORS error when frontend calls API"**
→ Frontend URL must be in `AllowedOrigins` list in backend `Program.cs`

**"Token expired"**
→ JWT tokens are set to 24h expiry; refresh token flow coming in next vertical slice

## Next steps

- Environment-specific config (dev/staging/prod)
- OAuth2 integration (login via LinkedIn, Google)
- Email notifications on job matches
- Search & filtering improvements
- Mobile app (React Native)