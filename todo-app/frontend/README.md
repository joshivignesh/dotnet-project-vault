# Todo App — Angular 21 + ASP.NET Core 10

A full stack Todo application built with the latest versions of Angular and .NET, demonstrating modern patterns including Signal-based reactivity, zoneless change detection, and minimal API design.

---

## Tech Stack

| Layer | Technology | Version |
|-------|-----------|---------|
| Frontend | Angular | 21 |
| Backend | ASP.NET Core Minimal API | .NET 10 |
| Database | Entity Framework Core InMemory | 10 |
| Language | TypeScript | 5.6 |
| Change Detection | Zoneless (no zone.js) | Angular 21 |

---

## Getting Started

### Backend

```bash
cd api/todo-api
dotnet run
# API runs at http://localhost:5000
# Swagger UI at http://localhost:5000/swagger
```

### Frontend

```bash
cd blazor/todo-angular
npm install
ng serve
# App runs at http://localhost:4200
```

---

## Why These Technology Choices

### Angular 21

Angular 21 is the current stable release (November 2025). It ships with Signals as a first-class reactive primitive, zoneless change detection as stable, and no zone.js by default. These are not experimental features anymore — they are the recommended way to build Angular apps going forward.

### Zoneless Change Detection

Previous Angular versions used zone.js to patch browser APIs and trigger change detection automatically. This worked but came with real costs — larger bundle size, harder debugging, noisy stack traces, and performance overhead from checking the entire component tree on every async event.

With zoneless, Angular only re-renders when a signal changes. The result is smaller bundles, cleaner stack traces, and predictable rendering behaviour.

```typescript
// main.ts — opting into zoneless
bootstrapApplication(AppComponent, {
  providers: [
    provideExperimentalZonelessChangeDetection(),
  ],
});
```

### Standalone Components

Angular 21 defaults to standalone components — no NgModules required. Each component declares its own imports, making it self-contained and easier to reason about.

### @if and @for Control Flow

The old `*ngIf` and `*ngFor` structural directives have been replaced with built-in control flow syntax. The new syntax is faster at runtime, easier to read, and does not require importing `CommonModule`.

```html
<!-- Old way -->
<li *ngFor="let todo of todos; trackBy: trackById">

<!-- New way — Angular 17+ -->
@for (todo of todos; track todo.id) {
  <li>{{ todo.title }}</li>
}
```

### ChangeDetectionStrategy.OnPush

Combined with signals, `OnPush` ensures a component only re-renders when its signal inputs change. Without this, Angular would still check the component on every parent render cycle.

---

## Signals vs Observables

This is the most important architectural decision in modern Angular. Here is a clear comparison:

### At a glance

| | Signals | Observables |
|--|---------|-------------|
| Import | `@angular/core` | `rxjs` |
| Read value | `mySignal()` | Must subscribe |
| Derived state | `computed()` | `map()`, `combineLatest()` |
| Side effects | `effect()` | `tap()`, `subscribe()` |
| HTTP calls | `httpResource()` | `HttpClient` + `async` pipe |
| Unsubscribe | Automatic | Manual (`takeUntil`, `async` pipe) |
| Learning curve | Low | High |
| Fine-grained rendering | Yes | No (requires `async` pipe tricks) |
| Best for | UI state, derived values | Complex async streams, WebSockets |

---

### Signals in this project

```typescript
// A writable signal — local state
readonly newTitle = signal('');

// A computed signal — derived automatically, no subscription needed
readonly completedCount = computed(() =>
  (this.todos.value() ?? []).filter(t => t.isComplete).length
);

// httpResource — signal-based HTTP, reloads reactively
readonly todos = httpResource<Todo[]>(this.apiUrl);

// Reading a signal in a template — just call it like a function
{{ todoService.pendingCount() }}
```

No `subscribe()`. No `unsubscribe()`. No memory leaks. The template re-renders only when the signal value changes.

---

### The equivalent with Observables

```typescript
// Observable approach — more ceremony
private destroy$ = new Subject<void>();

todos$: Observable<Todo[]>;
completedCount$: Observable<number>;

ngOnInit() {
  this.todos$ = this.http.get<Todo[]>(this.apiUrl);
  this.completedCount$ = this.todos$.pipe(
    map(todos => todos.filter(t => t.isComplete).length)
  );
}

ngOnDestroy() {
  this.destroy$.next();
  this.destroy$.complete();
}
```

And in the template you need the `async` pipe on every binding, which triggers separate subscriptions and can cause multiple HTTP requests if not shared.

---

### When to still use Observables

Signals do not replace Observables entirely. Observables are still the right tool when:

- Streaming data arrives over time — WebSockets, SSE, polling
- You need complex stream operators — `debounceTime`, `switchMap`, `mergeMap`
- You are combining multiple async sources with `combineLatest` or `forkJoin`
- You are working with existing RxJS-based libraries

Angular's `httpResource()` uses `HttpClient` under the hood, so interceptors, auth headers, and retry logic all continue to work exactly as before.

---

### The hybrid approach

Angular 21 ships `rxResource()` for cases where your data source is already an Observable:

```typescript
// Bridge between Observables and Signals
readonly todos = rxResource({
  loader: () => this.http.get<Todo[]>(this.apiUrl)
});

// Now todos.value() is a signal you can use directly in templates
```

This lets you use Signals in the template while keeping Observable-based services unchanged — a practical migration path for large codebases.

---

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/todos` | Get all todos |
| GET | `/todos/{id}` | Get by ID |
| POST | `/todos` | Create todo |
| PUT | `/todos/{id}` | Update todo |
| DELETE | `/todos/{id}` | Delete todo |

---

## Project Structure

```
blazor/todo-angular/
├── angular.json
├── package.json
├── tsconfig.json
└── src/
    ├── main.ts                          ← zoneless bootstrap
    └── app/
        ├── app.component.ts             ← root component, signals + @for/@if
        ├── models/
        │   └── todo.model.ts            ← Todo and CreateTodo interfaces
        └── services/
            └── todo.service.ts          ← httpResource, computed signals

api/todo-api/
├── Program.cs                           ← minimal API endpoints
├── TodoApi.csproj
├── Models/Todo.cs
└── Data/TodoDbContext.cs
```
