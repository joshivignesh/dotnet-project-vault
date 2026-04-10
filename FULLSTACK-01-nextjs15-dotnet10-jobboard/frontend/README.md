# Frontend

## Goal
Provide a Next.js 15 interface for job discovery, filtering, and application flows.

## Planned components
- Next.js app (TypeScript)
- UI layout and recruiter/candidate views
- API integration with backend

## Status
Minimal working frontend scaffold complete

## Current behavior
- Fetches jobs from GET /api/jobs
- Renders job cards on the landing page
- Shows a helpful message if backend is unavailable

## Run locally
1. Change to frontend app folder:
	- FULLSTACK-01-nextjs15-dotnet10-jobboard/frontend/web
2. Copy environment template if needed:
	- .env.example to .env.local
3. Install and start:
	- npm install
	- npm run dev