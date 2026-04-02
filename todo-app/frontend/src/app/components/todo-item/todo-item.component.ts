import { Component, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Todo } from '../../models/todo.model';

@Component({
  selector: 'app-todo-item',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="todo-item" [class.complete]="todo().isComplete">
      <input
        type="checkbox"
        [checked]="todo().isComplete"
        (change)="toggle.emit(todo())"
      />
      <span>{{ todo().title }}</span>
      <button class="remove-btn" (click)="remove.emit(todo().id)">✕</button>
    </div>
  `,
  styleUrl: './todo-item.component.css'
})
export class TodoItemComponent {
  // Signal-based input — replaces @Input() decorator (Angular 17+, stable in v20)
  readonly todo   = input.required<Todo>();

  // Signal-based output — replaces @Output() EventEmitter
  readonly toggle = output<Todo>();
  readonly remove = output<number>();
}
