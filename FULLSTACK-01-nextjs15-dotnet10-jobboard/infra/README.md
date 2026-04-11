# Infrastructure

## Local compose setup

This folder now contains a working docker compose definition for FULLSTACK-01:
- SQL Server 2022 container
- JobBoard API container
- Next.js web container

## Run
1. Change to this folder:
	- FULLSTACK-01-nextjs15-dotnet10-jobboard/infra
2. Start all services:
	- docker compose up --build
3. Open the app:
	- http://localhost:3000

## Service endpoints
- Web: http://localhost:3000
- API: http://localhost:5188
- SQL Server: localhost:1433

## Cloud deployment workflow
- Workflow file:
	- .github/workflows/fullstack01-deploy.yml
- Trigger:
	- Manual dispatch
	- Push to main when FULLSTACK-01 files change

## Required GitHub secrets
- AZURE_CLIENT_ID
- AZURE_TENANT_ID
- AZURE_SUBSCRIPTION_ID
- AZURE_RESOURCE_GROUP
- AZURE_LOCATION
- AZURE_CONTAINER_APP_NAME
- FULLSTACK01_DB_CONNECTION_STRING
- FULLSTACK01_JWT_KEY
- FULLSTACK01_FRONTEND_ORIGIN
- VERCEL_TOKEN
- VERCEL_ORG_ID
- VERCEL_PROJECT_ID
- FULLSTACK01_API_BASE_URL