# 🔷 dotnet-project-vault

A growing collection of .NET 10 projects — each one a self-contained full stack application with its own backend and frontend.

---

## 📁 Projects

| Project | Backend | Frontend | Description |
|---------|---------|----------|-------------|
| [todo-app](todo-app/) | ASP.NET Core 10 Minimal API | Angular 21 | Full stack CRUD with Signals and zoneless change detection |
| [weather-app](weather-app/) | ASP.NET Core 10 Minimal API | Angular 21 | OpenWeatherMap integration with caching and forecast |

> More coming — Auth API with JWT, SignalR chat app, gRPC microservice.

---

## 🗂 Structure

Each project is self-contained:

```
<project-name>/
├── backend/    ← ASP.NET Core .NET 10 API
└── frontend/   ← Angular 21 app
```

---

## ⚙️ Requirements

| Tool | Version |
|------|---------|
| .NET SDK | 10.0+ |
| Node.js | 20+ |
| Angular CLI | 21+ |

---

## 🚀 Running Any Project

```bash
# Backend
cd <project>/backend
dotnet run

# Frontend
cd <project>/frontend
npm install && ng serve
```

Each project folder has its own README with full setup instructions.
