import { Component, inject, ChangeDetectionStrategy } from '@angular/core';
import { TodoService } from '../services/todo.service';

@Component({
  selector: 'app-todo-list',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    @if (todoService.todos.isLoading()) {
      <p class="loading">Loading...</p>
    }

    @if (todoService.todos.error()) {
      <p class="error">Could not load todos. Is the API running?</p>
    }

    @if (!todoService.todos.isLoading() && (todoService.todos.value() ?? []).length === 0) {
      <p class="empty">No todos yet. Add one above!</p>
    }

    <ul class="todo-list">
      @for (todo of todoService.todos.value() ?? []; track todo.id) {
        <li class="todo-item" [class.done]="todo.isComplete">
          <label>
            <input
              type="checkbox"
              [checked]="todo.isComplete"
              (change)="toggle(todo.id)"
            />
            <span>{{ todo.title }}</span>
          </label>
          <button class="delete-btn" (click)="remove(todo.id)">✕</button>
        </li>
      }
    </ul>

    @if ((todoService.todos.value() ?? []).length > 0) {
      <p class="summary">
        {{ todoService.pendingCount() }} remaining ·
        {{ todoService.completedCount() }} completed
      </p>
    }
  `,
})
export class TodoListComponent {
  readonly todoService = inject(TodoService);

  toggle(id: number) {
    const todo = this.todoService.todos.value()?.find(t => t.id === id);
    if (!todo) return;
    this.todoService.toggle(id, todo).subscribe(() => this.todoService.todos.reload());
  }

  remove(id: number) {
    this.todoService.remove(id).subscribe(() => this.todoService.todos.reload());
  }
}
