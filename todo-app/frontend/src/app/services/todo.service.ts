import { Injectable, signal, computed, httpResource } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Todo, CreateTodo } from '../models/todo.model';

@Injectable({ providedIn: 'root' })
export class TodoService {
  private readonly apiUrl = 'http://localhost:5000/todos';
  private readonly http = inject(HttpClient);

  readonly todos = httpResource<Todo[]>(this.apiUrl);

  readonly completedCount = computed(() =>
    (this.todos.value() ?? []).filter(t => t.isComplete).length
  );

  readonly pendingCount = computed(() =>
    (this.todos.value() ?? []).filter(t => !t.isComplete).length
  );

  add(todo: CreateTodo) {
    return this.http.post<Todo>(this.apiUrl, todo);
  }

  toggle(id: number, todo: Todo) {
    return this.http.put<void>(`${this.apiUrl}/${id}`, {
      ...todo,
      isComplete: !todo.isComplete,
    });
  }

  remove(id: number) {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
