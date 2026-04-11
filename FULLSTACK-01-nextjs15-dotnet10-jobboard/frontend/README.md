# Frontend

## Goal
Provide a Next.js 15 interface for job discovery, filtering, and application flows.

## Planned components
- Next.js app (TypeScript)
- UI layout and recruiter/candidate views
- API integration with backend

## Status
Authenticated recruiter UI implemented for the first end-to-end vertical slice.

## Current behavior
- Shows login form and requests JWT from POST /api/auth/token
- Stores access token in local storage for local demo use
- Fetches jobs from secured GET /api/jobs with bearer token
- Allows creating jobs through secured POST /api/jobs

## Run locally
1. Change to frontend app folder:
	- FULLSTACK-01-nextjs15-dotnet10-jobboard/frontend/web
2. Copy environment template if needed:
	- .env.example to .env.local
3. Install and start:
	- npm install
	- npm run dev