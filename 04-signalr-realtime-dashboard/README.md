# 04 — Real-Time Metrics Dashboard with SignalR

![CI](https://github.com/vignesh-joshi-dotnet/dotnet-project-vault/actions/workflows/04-signalr-dashboard-ci.yml/badge.svg)
![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)
![Tests](https://img.shields.io/badge/tests-4%20passing-brightgreen)

## What this is

A real-time dashboard that shows live system metrics (CPU, memory, requests/sec) pushed from the server via SignalR. Open it in multiple browser tabs and watch them all update in sync — that's WebSocket magic.

No polling. No refresh buttons. Just live data.

## How it works

1. **SignalR hub** listens for client connections
2. **Background service** measures system metrics every 2 seconds
3. **Hub broadcasts** to all connected clients
4. **Browser updates** the dashboard without page reload

## See it in action

```bash
cd 04-signalr-realtime-dashboard
dotnet run --project src/SignalRDashboard.Api
# Open http://localhost:5000 in multiple browser tabs
# Watch them all update simultaneously
```

The dashboard shows:
- **CPU %** — current processor usage
- **Memory %** — RAM currently in use
- **Requests/sec** — API throughput
- **Avg response time** — latency in milliseconds
- **Active connections** — concurrent SignalR clients
- **Error rate** — failed requests as a percentage
- **Sparkline chart** — CPU trend over last 60 seconds

## Stack

| Component | Choice |
|-----------|--------|
| Real-time | ASP.NET Core SignalR (built-in) |
| Background work | `BackgroundService` base class |
| Frontend | Vanilla JavaScript + HTML5 Canvas |
| Styling | Dark theme, responsive layout |
| Tests | xUnit + SignalR test utilities |
| Container | Docker multi-stage |

## Tests

4 integration tests:

```bash
dotnet test
```

- Health endpoint returns OK
- SignalR negotiate endpoint works
- Static files served (index.html)
- Dashboard loads without errors

## Deployment

Docker:
```bash
docker build -t signalr-dashboard .
docker run -p 8080:8080 signalr-dashboard
```

Railway (one-click):
1. Fork repo → go to railway.app
2. Deploy from GitHub, point to this folder
3. Get live URL in 60 seconds

## Notes

- Open the same app in 2-3 browser tabs to see real-time sync behavior
- Metrics are simulated (not real system CPU) — customize `MetricsSimulatorService.cs` to read actual values
- SignalR auto-reconnects if the connection drops
- Works on any device with a modern browser (Firefox, Chrome, Safari, Edge)