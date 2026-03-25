import { Component, inject, signal, ChangeDetectionStrategy } from '@angular/core';
import { TodoService } from './services/todo.service';
import { CreateTodo } from './models/todo.model';

@Component({
  selector: 'app-root',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="container">
      <h1>Todo App</h1>

      <div class="stats">
        <span>Pending: {{ todoService.pendingCount() }}</span>
        <span>Done: {{ todoService.completedCount() }}</span>
      </div>

      <div class="add-todo">
        <input
          type="text"
          [value]="newTitle()"
          (input)="newTitle.set($any($event.target).value)"
          placeholder="What needs to be done?"
        />
        <button (click)="addTodo()">Add</button>
      </div>

      @if (todoService.todos.isLoading()) {
        <p>Loading...</p>
      }

      @if (todoService.todos.error()) {
        <p class="error">Failed to load todos</p>
      }

      <ul>
        @for (todo of todoService.todos.value() ?? []; track todo.id) {
          <li [class.done]="todo.isComplete">
            <span (click)="toggle(todo.id)">{{ todo.title }}</span>
            <button (click)="remove(todo.id)">✕</button>
          </li>
        }
      </ul>
    </div>
  `,
})
export class AppComponent {
  readonly todoService = inject(TodoService);
  readonly newTitle = signal('');

  addTodo() {
    const title = this.newTitle().trim();
    if (!title) return;
    const todo: CreateTodo = { title, isComplete: false };
    this.todoService.add(todo).subscribe(() => {
      this.newTitle.set('');
      this.todoService.todos.reload();
    });
  }

  toggle(id: number) {
    const todo = this.todoService.todos.value()?.find(t => t.id === id);
    if (!todo) return;
    this.todoService.toggle(id, todo).subscribe(() => this.todoService.todos.reload());
  }

  remove(id: number) {
    this.todoService.remove(id).subscribe(() => this.todoService.todos.reload());
  }
}
