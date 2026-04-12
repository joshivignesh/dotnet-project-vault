# 07 – Rate Limiting in ASP.NET Core 10

![CI](https://github.com/vignesh-joshi-dotnet/dotnet-project-vault/actions/workflows/07-rate-limiting-ci.yml/badge.svg)
![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)
![Tests](https://img.shields.io/badge/tests-8%20passing-brightgreen)

## Why you'd build this

Every public API needs rate limiting. But there's no one-size-fits-all algorithm. This project shows 4 different strategies (fixed-window, sliding-window, token-bucket, concurrency limiter) with real integration tests proving each one works.

## The algorithms

| Policy | How it works | Use it for |
|--------|--------------|-----------|
| **Fixed-window** | 5 requests per 10-second window | Strict public API throttling |
| **Sliding-window** | 20 requests per 30 seconds (with 3 segments) | Smoother, prevents boundary spikes |
| **Token-bucket** | 10 burst tokens, refill 2/sec | Tolerates short bursts (searches, reads) |
| **Concurrency** | Max 3 requests at the same time | Heavy workloads (compute, DB queries) |

Visual comparison:
```
Fixed Window hits a cliff when the window rolls over.
Sliding Window smooths that out.
Token Bucket lets you burst, then refills over time.
Concurrency says "only N things running at once."
```

## Endpoints

| Method | Path | Policy | Description |
|--------|------|--------|-------------|
| `GET` | `/health` | none | Health check — never throttled |
| `GET` | `/api/public/data` | fixed-window | Public API — 5 req / 10 s |
| `GET` | `/api/reports/summary` | sliding-window | Reporting — 20 req / 30 s |
| `GET` | `/api/search?q=` | token-bucket | Search — 10 burst, +2/s |
| `POST` | `/api/compute` | concurrency | Compute — max 3 concurrent |

All `429 Too Many Requests` responses return:

```json
{
  "error": "Too many requests",
  "retryAfterSeconds": 7
}
```

Plus a `Retry-After` header when the lease provides metadata.

## Architecture

```
HTTP Client
    │
    ▼
RateLimiter Middleware  ◄── policy dispatch by endpoint name
    │                            ├─ fixed-window   (public data)
    │                            ├─ sliding-window (reports)
    │                            ├─ token-bucket   (search)
    │                            └─ concurrency    (compute)
    ▼
Minimal API Endpoints
    │
    ├─ /health           → always passes through
    ├─ /api/public/data
    ├─ /api/reports/summary
    ├─ /api/search
    └─ /api/compute
```

## Quick Start

### Option 1 — dotnet run
```bash
cd 07-rate-limiting-middleware
dotnet run --project src/RateLimitingMiddleware.Api
# API at http://localhost:5000
```

Explore the OpenAPI schema at `http://localhost:5000/openapi/v1.json`.

### Option 2 — Docker
```bash
cd 07-rate-limiting-middleware
docker build -t rate-limiting-demo .
docker run -p 8080:8080 rate-limiting-demo
# API at http://localhost:8080
```

### Option 3 — Test the rate limits manually
```bash
# Trigger the fixed-window limit (run 6 times quickly)
for i in {1..6}; do curl -s -o /dev/null -w "%{http_code}\n" http://localhost:5000/api/public/data; done
# Expected: 200 200 200 200 200 429
```

## Tests

8 integration tests, isolated with one `WebApplicationFactory<Program>` per test class to prevent shared limiter state:

```
RateLimitingMiddleware.Tests
  HealthTests
    ✅ Health_NeverRateLimited_AlwaysReturns200
    ✅ Health_ReturnsJsonContentType
  FixedWindowTests
    ✅ FixedWindow_Allows5ThenRejects
    ✅ FixedWindow_RejectedResponse_HasJsonErrorBody
  SlidingWindowTests
    ✅ SlidingWindow_Allows20Requests
  TokenBucketTests
    ✅ TokenBucket_Allows10BurstThenRejects
  ConcurrencyTests
    ✅ Concurrency_SingleRequest_Returns200
```

```bash
cd 07-rate-limiting-middleware
dotnet test --verbosity normal
```

## Deployment Notes

### Container Ports
- Listens on port `8080` inside the container
- `ASPNETCORE_URLS=http://+:8080` set in the Dockerfile

### Environment Variables
| Variable | Default | Purpose |
|----------|---------|---------|
| `ASPNETCORE_ENVIRONMENT` | `Production` | Controls OpenAPI visibility |
| `ASPNETCORE_URLS` | `http://+:8080` | Binding address |

### Azure Container Apps
```bash
az containerapp create \
  --name rate-limiting-demo \
  --resource-group my-rg \
  --environment my-env \
  --image myregistry.azurecr.io/rate-limiting-demo:latest \
  --target-port 8080 \
  --ingress external
```

## Key Implementation Notes

- **No auth-based per-user limits here** — add `httpContext.User.Identity?.Name` as a partition key in each policy's `PartitionKey` lambda for per-user limiting
- **Queue behaviour** — fixed-window has `QueueLimit = 0` (immediate reject); token-bucket also `QueueLimit = 0`; sliding-window and concurrency allow small queues to absorb short spikes
- **`OnRejected` callback** — global callback; writes JSON body + `Retry-After` header when lease metadata is available
- **WebApplicationFactory isolation** — rate limiter is a singleton so shared test factories contaminate counts; each test class owns its own factory instance

## TODOs

- [ ] Add per-IP partitioned limiting (add `PartitionKey` lambda using `RemoteIpAddress`)
- [ ] Add Redis-backed distributed rate limiting for multi-instance deployments
- [ ] Add Prometheus metrics for `rate_limit_rejected_total` counter


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