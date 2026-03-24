# Todo App — Angular 21 + ASP.NET Core 10

A full-stack Todo app built with the latest Angular and .NET stack.
The frontend uses Angular Signals throughout — this README explains every tech decision and why Signals were chosen over Observables for state management.

---

## Stack

| Layer | Technology | Version |
|-------|-----------|---------|
| Frontend | Angular | 21 |
| Backend | ASP.NET Core Minimal API | .NET 10 |
| Language | TypeScript | 5.6 |
| HTTP | Angular HttpClient + RxJS | 7.8 |
| State | Angular Signals | Stable (v20+) |

---

## Why Angular 21

Angular 21 is the latest release (November 2025). Key reasons for choosing it:

**Standalone components are the default** — no NgModules needed. Every component in this project uses `standalone: true`, which means no `app.module.ts`, no `declarations` array, no ceremony. You import exactly what a component needs, right in the component file itself.

**Signals are stable** — all reactivity primitives (`signal`, `computed`, `effect`, `input`, `output`) graduated to stable in Angular 20 and are the recommended approach in Angular 21. This project uses them throughout.

**`@for` and `@if` control flow** — the new built-in control flow syntax replaces `*ngFor` and `*ngIf` directives. It is more readable, has better type narrowing, and the `@empty` block on `@for` removes the need for an extra `@if` to handle empty lists.

**Signal-based `input()` and `output()`** — replaces `@Input()` and `@Output()` decorators. Components declare what they need at the class level instead of using decorators, making the data flow more explicit and easier to follow.

---

## Signals vs Observables

This is the most important architectural decision in the project. Here is a direct comparison.

### What is a Signal?

A Signal is a value that Angular tracks. When it changes, only the parts of the UI that read it are updated — no change detection cycle needed, no subscription management.

```typescript
// Create a signal
const count = signal(0);

// Read it — call it like a function
console.log(count()); // 0

// Update it
count.set(1);
count.update(v => v + 1);

// Derive new values — recomputes automatically when count changes
const doubled = computed(() => count() * 2);
```

### What is an Observable?

An Observable represents a stream of values over time. It is lazy — nothing happens until something subscribes to it. It is the right tool for async data flows, event streams, and complex transformations.

```typescript
// Observable — lazy, async, needs subscription
this.http.get<Todo[]>('/todos').subscribe(todos => {
  this.todos = todos;
});
```

### Side by Side Comparison

| | Signals | Observables (RxJS) |
|---|---|---|
| **Best for** | Synchronous UI state | Async data streams, HTTP, events |
| **Reading a value** | `todos()` — call it | Must subscribe or use `async` pipe |
| **Updating** | `todos.set([...])` | Emit a new value from a Subject |
| **Derived values** | `computed(() => ...)` | `pipe(map(...))` |
| **Side effects** | `effect(() => ...)` | `tap(...)` in a pipe |
| **Subscription management** | None — Angular handles it | Must unsubscribe to avoid leaks |
| **Change detection** | Fine-grained — only re-renders what changed | Zone.js detects change after async ops |
| **Learning curve** | Low — reads like plain variables | Higher — operators, marble diagrams |
| **Async support** | No native async (use `resource()` for HTTP) | First-class — built for async |

### How this project uses both

This project deliberately uses both, each where it is strongest:

**Signals are used for state** — the list of todos, loading flag, error message, and derived counts (`totalCount`, `doneCount`, `pendingCount`) are all signals in `TodoService`. The template reads them directly — no `async` pipe, no subscription, no memory leaks.

```typescript
// Service — state as signals
private readonly _todos = signal<Todo[]>([]);
readonly totalCount   = computed(() => this._todos().length);
readonly doneCount    = computed(() => this._todos().filter(t => t.isComplete).length);
```

**Observables are used for HTTP** — `HttpClient` returns Observables. That is intentional. HTTP is async, can fail, can be cancelled, and can be composed. RxJS `tap` is used to update the signal when the HTTP call succeeds.

```typescript
// HTTP returns Observable — update signal on success via tap
loadAll(): Observable<Todo[]> {
  return this.http.get<Todo[]>(this.apiUrl).pipe(
    tap(todos => this._todos.set(todos))
  );
}
```

**The pattern:** HTTP (Observable) fetches data → on success, `tap` writes to a Signal → template reads the Signal. The two systems complement each other cleanly.

### When to use which

Use **Signals** when:
- Managing component or service state
- Deriving values from other state (`computed`)
- Replacing `BehaviorSubject` + `async` pipe patterns
- You want fine-grained reactivity without zone.js

Use **Observables** when:
- Making HTTP requests
- Listening to DOM events over time
- Combining multiple async streams (`combineLatest`, `switchMap`)
- You need cancellation or retry logic

---

## Project Structure

```
todo-angular/
├── src/
│   ├── main.ts                          ← bootstraps the app, provides HttpClient + Router
│   └── app/
│       ├── app.component.ts             ← root component, imports TodoListComponent
│       ├── models/
│       │   └── todo.model.ts            ← Todo interface
│       ├── services/
│       │   └── todo.service.ts          ← signals state + HTTP via Observables
│       └── components/
│           ├── todo-list/               ← reads service signals, handles user input
│           └── todo-item/               ← signal input(), signal output()
└── package.json
```

---

## Running the App

```bash
# Install dependencies
npm install

# Start Angular dev server (connects to backend on :5000)
npm start
```

Make sure the ASP.NET Core API in `api/todo-api/` is running on port 5000 first.

---

## Key Angular 21 APIs Used

| API | Replaces | Why |
|-----|---------|-----|
| `signal()` | `BehaviorSubject` | Simpler state, no subscription needed |
| `computed()` | `pipe(map(...))` for derived state | Lazily recomputes only when dependencies change |
| `input.required<T>()` | `@Input()` decorator | Signal-based, type-safe, no decorator magic |
| `output<T>()` | `@Output() EventEmitter` | Consistent with signal model |
| `inject()` | Constructor injection | Works outside constructors, cleaner code |
| `@for ... @empty` | `*ngFor` + extra `*ngIf` | Built-in, better type narrowing |
| `@if` | `*ngIf` | Built-in control flow, no structural directive import |
| `standalone: true` | NgModule declarations | Import only what you need, per component |
