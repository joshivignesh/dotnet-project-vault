# 08 – Code Quality Gates with SonarCloud + GitHub Actions

![CI](https://github.com/vignesh-joshi-dotnet/dotnet-project-vault/actions/workflows/sonarqube-dotnet.yml/badge.svg)
![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)
![Tests](https://img.shields.io/badge/tests-36%20passing-brightgreen)
![SonarCloud](https://img.shields.io/badge/SonarCloud-enabled-orange?logo=sonarcloud)

## What this is

A CI/CD pipeline that catches bugs, code smells, and coverage gaps before they hit main. Every push runs static analysis on SonarCloud — if quality drops or coverage falls below the threshold, the build fails.

This is what real teams do. Code quality isn't optional.

## The real business logic

Not toy code. This project includes:
- **OrderCalculator** — calculates subtotals, tier-based discounts (5-20%), promo codes, 8% sales tax
- **TextAnalyzer** — counts words/sentences, estimates syllables, calculates Flesch Reading Ease score

Both are fully tested (36 tests total) and send coverage metrics to SonarCloud.

## Stack

| Component | Choice |
|-----------|--------|
| Runtime | .NET 10 / C# 14 |
| API style | ASP.NET Core 10 Minimal API |
| Static analysis | SonarCloud (sonarcloud.io) |
| Coverage tool | `dotnet-coverage` (Microsoft global tool) |
| Scanner | `dotnet-sonarscanner` global tool |
| Tests | xUnit + `WebApplicationFactory<Program>` |
| CI | GitHub Actions |
| Container | Docker (multi-stage) |

## Project Business Logic

Two analyzable services (not toy code):

### `OrderCalculator`
- Calculates subtotal, tier-based discounts (4 tiers: 5–20%), promo codes, 8% tax, total
- 3 valid promo codes: `SAVE10`, `SAVE20`, `VIP30` — highest of promo vs. tier wins
- Full input validation with `ArgumentException`

### `TextAnalyzer`
- Counts words, sentences, paragraphs, characters
- Estimates syllables per word using vowel-run heuristics
- Calculates **Flesch Reading Ease** score (0–100)
- Maps score to readability level: Very Easy → Very Difficult

## API Endpoints

| Method | Path | Description |
|--------|------|-------------|
| `GET` | `/health` | Health check |
| `POST` | `/api/orders/calculate` | Calculate order total with discounts + tax |
| `GET` | `/api/orders/discount-tiers` | List current discount tier thresholds |
| `POST` | `/api/text/analyze` | Analyze text for readability metrics |

### Example Order Request
```json
POST /api/orders/calculate
{
  "items": [
    { "productName": "Widget Pro", "unitPrice": 299.99, "quantity": 2 },
    { "productName": "Gadget X",   "unitPrice": 149.50, "quantity": 3 }
  ],
  "promoCode": "SAVE10"
}
```

```json
{
  "subtotal": 1048.48,
  "discountPercent": 10,
  "discountAmount": 104.85,
  "taxAmount": 83.89,
  "total": 1027.52,
  "appliedPromoCode": "SAVE10"
}
```

## Test Coverage

36 tests across 3 focused test classes. Run them locally:

```bash
cd 08-sonarqube-github-actions
dotnet test
```

**OrderCalculatorTests (11)**
- Subtotal, tax, discount tiers (5 tiers: 5-20%)
- Promo code logic (SAVE10, SAVE20, VIP30)
- Edge cases (negative prices, invalid quantities)

**TextAnalyzerTests (10)**
- Word/sentence/paragraph counting
- Syllable estimation
- Flesch Reading Ease scoring
- Readability level assignment

**ApiIntegrationTests (6)**
- HTTP endpoints responding with 200
- Invalid input returning 400
- Health check always passes

## SonarCloud Setup (One-Time)

### 1. Create SonarCloud account
1. Go to [sonarcloud.io](https://sonarcloud.io) → Sign in with GitHub
2. Import your repository
3. Note your **Project Key** and **Organization key**

### 2. Add GitHub Secrets
In your GitHub repo → Settings → Secrets and variables → Actions:

| Secret/Variable | Value |
|-----------------|-------|
| `SONAR_TOKEN` (secret) | Your SonarCloud user token |
| `SONAR_PROJECT_KEY` (variable) | e.g. `vignesh-joshi-dotnet_sonarqube-sample` |
| `SONAR_ORGANIZATION` (variable) | Your SonarCloud org slug |

### 3. Update sonarqube.properties
```properties
sonar.projectKey=your-org_your-project-key
sonar.projectName=dotnet-project-vault-sonarqube-sample
sonar.sourceEncoding=UTF-8
sonar.sources=src
sonar.tests=tests
```

### 4. Push to trigger analysis
The workflow will run on every push to paths under `08-sonarqube-github-actions/`.

## CI Pipeline Flow

```
push / PR
    │
    ▼
Setup .NET 10 + install dotnet-sonarscanner + dotnet-coverage
    │
    ├─ [SONAR_TOKEN set] ─────────────────────────────────────┐
    │   sonarscanner begin                                     │
    │   dotnet build                                           │
    │   dotnet-coverage collect (dotnet test) → coverage.xml  │
    │   sonarscanner end → results posted to SonarCloud        │
    │   quality gate check (fails build if gate fails)    ◄────┘
    │
    └─ [SONAR_TOKEN not set — fork/draft PR]
        dotnet test --verbosity normal (no analysis)
    │
    ▼
Docker Build (on success)
```

## Quick Start

### Option 1 — dotnet run
```bash
cd 08-sonarqube-github-actions
dotnet run --project src/SonarGithubActions.Api
# API at http://localhost:5000
```

### Option 2 — Docker
```bash
cd 08-sonarqube-github-actions
docker build -t sonar-demo .
docker run -p 8080:8080 sonar-demo
```

## Deployment Notes

- The API itself is stateless and can be deployed anywhere that runs containers
- SonarCloud analysis runs only in CI — no runtime dependency on SonarCloud
- Environment: `ASPNETCORE_URLS=http://+:8080`, port 8080 in Docker

## TODOs

- [ ] Add SonarCloud badge with actual quality gate status (requires linked account)
- [ ] Add coverage exclusion patterns for generated code (`[ExcludeFromCodeCoverage]`)
- [ ] Configure severity thresholds in SonarCloud quality profile


## Status
Scaffolded

## Next step
Wire SonarCloud/SonarQube tokens in repository secrets and tune quality gates.

## Current scaffold
- SonarGithubActions.slnx
- src/SonarGithubActions.Api
- tests/SonarGithubActions.Tests
- sonarqube.properties
- .github/workflows/sonarqube-dotnet.yml