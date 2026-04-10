# Weather App — Full Stack

A full stack weather application built with ASP.NET Core 10 Minimal API and Angular 21. Fetches real-time weather and 5-day forecasts from OpenWeatherMap, with signal-based reactivity, search history, and °C / °F unit conversion.

---

## Stack

| Layer | Technology | Version |
|-------|-----------|---------|
| Backend | ASP.NET Core Minimal API | .NET 10 |
| Frontend | Angular | 21 |
| Change Detection | Zoneless (no zone.js) | Angular 21 |
| HTTP (backend) | Typed HttpClient + IHttpClientFactory | .NET 10 |
| Caching (backend) | IMemoryCache | .NET 10 |
| Reactivity (frontend) | Signals — signal, computed, rxResource | Angular 21 |
| Weather data | OpenWeatherMap API | v2.5 |

---

## Getting Started

### 1. Get an API key

Sign up free at [openweathermap.org](https://openweathermap.org/api). The free tier covers both endpoints used here.

### 2. Configure the API key

```bash
cd weather-app/backend
dotnet user-secrets set "OpenWeatherMap:ApiKey" "your_key_here"
```

Or edit `appsettings.json` directly for local dev:

```json
{
  "OpenWeatherMap": {
    "ApiKey": "your_key_here"
  }
}
```

### 3. Run the backend

```bash
cd weather-app/backend
dotnet run
# API → http://localhost:5000
# Swagger → http://localhost:5000/swagger
```

### 4. Run the frontend

```bash
cd weather-app/frontend
npm install
ng serve
# App → http://localhost:4200
```

---

## Features

- **Current weather** — temperature, feels like, min/max, humidity, wind, visibility, sunrise/sunset
- **5-day forecast** — one entry per day, deduped from 3-hour interval data
- **Search history** — last 5 cities stored in a signal, clickable chips to re-search
- **°C / °F toggle** — unit conversion applied instantly across all components via a single signal
- **Error handling** — city not found shown inline, API errors surfaced cleanly

---

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/weather/{city}` | Current weather |
| GET | `/weather/{city}/forecast` | 5-day forecast |

Both endpoints return `404` if the city is not found, and cache results to avoid hammering the OpenWeatherMap rate limit.

---

## How Signals Drive the Frontend

The entire frontend state lives in three signals inside `WeatherService`:

```typescript
readonly city    = signal('');            // which city to fetch
readonly unit    = signal<'C' | 'F'>('C'); // temperature unit
readonly history = signal<string[]>([]);  // recent searches
```

`rxResource` watches `city` and re-fetches automatically whenever it changes:

```typescript
readonly weather = rxResource<WeatherSummary, string>({
  request: () => this.city(),
  loader: ({ request: city }) =>
    this.http.get<WeatherSummary>(`${this.apiUrl}/weather/${city}`),
});
```

When the user toggles °C / °F, `unit` changes and every template binding that calls `svc.convertTemp()` re-evaluates — with no subscriptions, no manual `markForCheck()`, and no zone.js.

```typescript
toggleUnit() {
  this.unit.update(u => u === 'C' ? 'F' : 'C');
}

convertTemp(celsius: number): number {
  return this.unit() === 'F'
    ? Math.round((celsius * 9 / 5) + 32)
    : Math.round(celsius);
}
```

Search history deduplicates and caps at 5 entries using `signal.update()`:

```typescript
private addToHistory(city: string) {
  this.history.update(prev => {
    const filtered = prev.filter(c => c.toLowerCase() !== city.toLowerCase());
    return [city, ...filtered].slice(0, 5);
  });
}
```

---

## Project Structure

```
weather-app/
├── backend/
│   ├── Program.cs                ← Minimal API endpoints, DI wiring
│   ├── WeatherApi.csproj         ← .NET 10, Memory Cache, Swagger
│   ├── appsettings.json          ← API key, base URL, cache durations
│   ├── Models/
│   │   ├── WeatherResponse.cs    ← Typed OpenWeatherMap response records
│   │   └── WeatherSummary.cs     ← Clean DTOs returned to the client
│   └── Services/
│       └── WeatherService.cs     ← HttpClient + IMemoryCache + mapping
│
└── frontend/
    ├── package.json
    ├── angular.json
    ├── tsconfig.json
    └── src/
        ├── main.ts               ← Zoneless bootstrap, no zone.js
        ├── styles.css
        └── app/
            ├── app.component.ts  ← Search input, wires all components
            ├── models/
            │   └── weather.model.ts
            ├── services/
            │   └── weather.service.ts  ← city, unit, history signals
            └── components/
                ├── weather-card.component.ts   ← Current weather + toggle
                ├── forecast.component.ts       ← 5-day grid
                └── search-history.component.ts ← Recent city chips
```

---

## Backend Design Decisions

### Typed HttpClient

Registered via `AddHttpClient<IWeatherService, WeatherService>()` — managed by `IHttpClientFactory`, avoiding socket exhaustion from manually instantiated clients.

### IMemoryCache

Protects the OpenWeatherMap free tier rate limit. Current weather cached for 10 minutes, forecast for 30 minutes — both configurable in `appsettings.json`.

### Record types

All models use C# `record` — immutable by default, structural equality, and no boilerplate. The raw `WeatherResponse` maps the API shape; `WeatherSummary` is the clean flat DTO the client receives. Decoupling these means API shape changes only require updating one mapping function.

---

## What to Add Next

- Persist search history to `localStorage` via an Angular service
- Geolocation — detect user's city on first load
- Polly retry policy on the backend for transient OpenWeatherMap failures
- Weather icons with animated SVGs instead of OpenWeatherMap PNGs
- Unit tests — Jasmine/Vitest for Angular components, xUnit for the API
