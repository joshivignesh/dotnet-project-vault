# 🔷 dotnet-project-vault

A growing collection of .NET 10 projects — from console basics to full stack applications with Angular, Blazor, microservices, and Azure integrations.

Every project is self-contained with its own README, focused on using the latest stable APIs and patterns.

---

## 📁 Projects

| Project | Stack | Description |
|---------|-------|-------------|
| [Todo App](blazor/todo-angular/) | ASP.NET Core 10 + Angular 21 | Full stack CRUD app with Signal-based reactivity and zoneless change detection |

> More projects coming — Blazor WASM, gRPC microservices, Azure Functions, and more.

---

## 🗂 Structure

```
dotnet-project-vault/
├── basics/          ← C# fundamentals and console apps
├── api/             ← ASP.NET Core Minimal API projects
├── blazor/          ← Angular and Blazor frontend projects
├── microservices/   ← gRPC and distributed service examples
└── azure/           ← Azure SDK, Functions, and cloud integrations
```

---

## ⚙️ Requirements

| Tool | Version |
|------|---------|
| .NET SDK | 10.0+ |
| Node.js | 20+ |
| Angular CLI | 21+ |

```bash
# Check your versions
dotnet --version
node --version
ng version
```

---

## 🚀 Running Any Project

Each project folder has its own README with setup instructions. The general pattern is:

```bash
# Backend
cd api/<project-name>
dotnet run

# Frontend
cd blazor/<project-name>
npm install && ng serve
```

---

## 🤝 Contributing

Each new project should follow the existing folder structure and include its own `README.md` explaining what was built and why the technology choices were made.
