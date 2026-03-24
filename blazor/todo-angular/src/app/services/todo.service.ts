import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { Todo } from '../models/todo.model';

@Injectable({ providedIn: 'root' })
export class TodoService {
  private readonly apiUrl = 'http://localhost:5000/todos';

  // Signals — single source of truth for UI state
  private readonly _todos   = signal<Todo[]>([]);
  private readonly _loading = signal<boolean>(false);
  private readonly _error   = signal<string | null>(null);

  // Public readonly computed signals — components read these, never mutate directly
  readonly todos      = this._todos.asReadonly();
  readonly loading    = this._loading.asReadonly();
  readonly error      = this._error.asReadonly();
  readonly totalCount = computed(() => this._todos().length);
  readonly doneCount  = computed(() => this._todos().filter(t => t.isComplete).length);
  readonly pendingCount = computed(() => this.totalCount() - this.doneCount());

  constructor(private http: HttpClient) {}

  // Observable used for HTTP — async data streams are still RxJS's strength
  loadAll(): Observable<Todo[]> {
    this._loading.set(true);
    return this.http.get<Todo[]>(this.apiUrl).pipe(
      tap({
        next: todos => { this._todos.set(todos); this._loading.set(false); },
        error: err  => { this._error.set(err.message); this._loading.set(false); }
      })
    );
  }

  add(title: string): Observable<Todo> {
    const body: Partial<Todo> = { title, isComplete: false };
    return this.http.post<Todo>(this.apiUrl, body).pipe(
      tap(created => this._todos.update(list => [...list, created]))
    );
  }

  toggle(todo: Todo): Observable<void> {
    const updated = { ...todo, isComplete: !todo.isComplete };
    return this.http.put<void>(`${this.apiUrl}/${todo.id}`, updated).pipe(
      tap(() =>
        this._todos.update(list =>
          list.map(t => (t.id === todo.id ? updated : t))
        )
      )
    );
  }

  remove(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`).pipe(
      tap(() => this._todos.update(list => list.filter(t => t.id !== id)))
    );
  }
}
