# Weather API — ASP.NET Core 10 Minimal API

A clean, production-aware Weather API built with ASP.NET Core 10, consuming the OpenWeatherMap API. Demonstrates typed HTTP clients, memory caching, structured configuration, and minimal API endpoint design.

---

## Getting Started

### 1. Get an API key

Sign up at [openweathermap.org](https://openweathermap.org/api) — the free tier covers both endpoints used here.

### 2. Add your API key

Open `appsettings.json` and replace the placeholder:

```json
{
  "OpenWeatherMap": {
    "ApiKey": "YOUR_API_KEY_HERE"
  }
}
```

For production, use environment variables or user secrets instead:

```bash
dotnet user-secrets set "OpenWeatherMap:ApiKey" "your_key_here"
```

### 3. Run

```bash
cd api/weather-api
dotnet run
# API → http://localhost:5000
# Swagger → http://localhost:5000/swagger
```

---

## Endpoints

### `GET /weather/{city}`

Returns current weather for a city.

```bash
curl http://localhost:5000/weather/London
```

```json
{
  "city": "London",
  "country": "GB",
  "temperature": 14.2,
  "feelsLike": 13.1,
  "tempMin": 12.0,
  "tempMax": 16.5,
  "humidity": 72,
  "condition": "Clouds",
  "description": "overcast clouds",
  "icon": "04d",
  "windSpeed": 5.1,
  "visibility": 10000,
  "sunrise": "2026-03-21T06:15:00Z",
  "sunset": "2026-03-21T18:30:00Z",
  "fetchedAt": "2026-03-21T09:00:00Z"
}
```

### `GET /weather/{city}/forecast`

Returns a 5-day, 3-hour interval forecast.

```bash
curl http://localhost:5000/weather/Tokyo/forecast
```

```json
{
  "city": "Tokyo",
  "country": "JP",
  "items": [
    {
      "dateTime": "2026-03-21T12:00:00Z",
      "temperature": 18.4,
      "condition": "Clear",
      "description": "clear sky",
      "icon": "01d",
      "windSpeed": 3.2,
      "humidity": 55
    }
  ]
}
```

### Error responses

| Scenario | Status | Body |
|----------|--------|------|
| City not found | 404 | `{ "error": "City 'xyz' not found." }` |
| OpenWeatherMap unreachable | 502 | Standard ASP.NET error response |
| Invalid API key | 401 | Standard ASP.NET error response |

---

## Architecture Decisions

### Typed HttpClient

ASP.NET Core's `AddHttpClient<TInterface, TImpl>()` registers a named, typed HTTP client managed by `IHttpClientFactory`. This is the recommended approach over creating `HttpClient` manually.

```csharp
builder.Services.AddHttpClient<IWeatherService, WeatherService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["OpenWeatherMap:BaseUrl"]!);
    client.Timeout = TimeSpan.FromSeconds(10);
});
```

Benefits over `new HttpClient()`:
- Manages connection lifetime — avoids socket exhaustion
- Integrates with DI — no static instances
- Supports Polly for retry policies with one line

### IMemoryCache — avoiding API rate limits

OpenWeatherMap's free tier allows 60 calls per minute. Without caching, a busy frontend hitting the same city repeatedly would exhaust that quota fast.

```csharp
// Check cache first
if (cache.TryGetValue(cacheKey, out WeatherSummary? cached))
    return cached;

// Fetch from API, then store
cache.Set(cacheKey, summary, TimeSpan.FromMinutes(10));
```

Cache durations are configurable in `appsettings.json` — no hardcoded values in the service.

| Endpoint | Default cache duration | Reason |
|----------|----------------------|--------|
| Current weather | 10 minutes | Changes slowly |
| Forecast | 30 minutes | 3-hour intervals — no point refreshing sooner |

### C# Records for models

All models use `record` types instead of classes. Records are immutable by default, support structural equality, and work perfectly for HTTP response mapping where you never need to mutate the data.

```csharp
public record WeatherSummary(
    string City,
    string Country,
    double Temperature,
    ...
);
```

### Clean DTO separation

OpenWeatherMap's response shape is nested and verbose. `WeatherResponse` maps the raw API shape. `WeatherSummary` is the clean, flat DTO the client actually receives.

This means if OpenWeatherMap changes their response format, you only update `WeatherResponse` and `MapToSummary` — the rest of the codebase is unaffected.

```
OpenWeatherMap JSON → WeatherResponse → MapToSummary() → WeatherSummary → client
```

### Configuration with IConfiguration

API keys and cache durations live in `appsettings.json`, not in code. The service reads them via `IConfiguration` injected through the constructor.

For a larger project, use `IOptions<T>` with a typed settings class:

```csharp
// Typed settings class (next improvement)
public record OpenWeatherMapSettings(string ApiKey, string BaseUrl, string Units);
builder.Services.Configure<OpenWeatherMapSettings>(
    builder.Configuration.GetSection("OpenWeatherMap"));
```

---

## Project Structure

```
api/weather-api/
├── WeatherApi.csproj        ← .NET 10, Memory Cache, Swagger
├── Program.cs               ← DI wiring, endpoints
├── appsettings.json         ← API key, base URL, cache config
├── Models/
│   ├── WeatherResponse.cs   ← typed OpenWeatherMap response (records)
│   └── WeatherSummary.cs    ← clean DTOs returned to Angular / client
└── Services/
    └── WeatherService.cs    ← HttpClient + IMemoryCache + mapping logic
```

---

## What to Add Next

- **Polly retry policy** — retry transient failures automatically
- **`IOptions<T>`** — replace `IConfiguration` with strongly typed settings class
- **Response compression** — `AddResponseCompression()` for faster payloads
- **Health check endpoint** — `app.MapHealthChecks("/health")`
- **Angular frontend** — signal-based weather dashboard consuming these endpoints
