import { Component, inject, signal, ChangeDetectionStrategy } from '@angular/core';
import { TodoService } from '../services/todo.service';
import { CreateTodo } from '../models/todo.model';

@Component({
  selector: 'app-todo-form',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <form class="todo-form" (ngSubmit)="submit()">
      <input
        class="todo-input"
        type="text"
        placeholder="What needs to be done?"
        [value]="title()"
        (input)="title.set($any($event.target).value)"
      />
      <button
        class="add-btn"
        type="submit"
        [disabled]="!title().trim()"
      >
        Add
      </button>
    </form>

    @if (error()) {
      <p class="error">{{ error() }}</p>
    }
  `,
})
export class TodoFormComponent {
  readonly todoService = inject(TodoService);
  readonly title = signal('');
  readonly error = signal('');

  submit() {
    const trimmed = this.title().trim();
    if (!trimmed) return;

    const todo: CreateTodo = { title: trimmed, isComplete: false };

    this.todoService.add(todo).subscribe({
      next: () => {
        this.title.set('');
        this.error.set('');
        this.todoService.todos.reload();
      },
      error: () => this.error.set('Failed to add todo. Please try again.'),
    });
  }
}
