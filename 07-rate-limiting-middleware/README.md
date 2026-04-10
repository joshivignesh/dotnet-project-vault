# 07-rate-limiting-middleware

## Purpose
Planned middleware-focused sample demonstrating request throttling and policy-driven limits.

## Stack
- .NET 10
- C# 14
- ASP.NET Core 10

## Status
Scaffolded

## Next step
Add policy variants per route and add integration tests for throttling behavior.

## Current scaffold
- RateLimitingMiddleware.slnx
- src/RateLimitingMiddleware.Api
- tests/RateLimitingMiddleware.Tests

## Current endpoints
- GET /health
- GET /api/ping (fixed-window rate limited)