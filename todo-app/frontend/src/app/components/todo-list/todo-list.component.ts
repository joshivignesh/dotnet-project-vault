import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TodoService } from '../../services/todo.service';
import { TodoItemComponent } from '../todo-item/todo-item.component';

@Component({
  selector: 'app-todo-list',
  standalone: true,
  imports: [CommonModule, FormsModule, TodoItemComponent],
  template: `
    <div class="container">
      <h1>Todo App</h1>

      <div class="stats">
        <span>Total: {{ service.totalCount() }}</span>
        <span>Done: {{ service.doneCount() }}</span>
        <span>Pending: {{ service.pendingCount() }}</span>
      </div>

      <div class="add-row">
        <input
          [(ngModel)]="newTitle"
          (keyup.enter)="add()"
          placeholder="What needs doing?"
        />
        <button (click)="add()" [disabled]="!newTitle().trim()">Add</button>
      </div>

      @if (service.loading()) {
        <p class="status">Loading...</p>
      }

      @if (service.error()) {
        <p class="error">{{ service.error() }}</p>
      }

      @for (todo of service.todos(); track todo.id) {
        <app-todo-item
          [todo]="todo"
          (toggle)="service.toggle($event).subscribe()"
          (remove)="service.remove($event).subscribe()"
        />
      } @empty {
        <p class="status">No todos yet — add one above.</p>
      }
    </div>
  `,
  styleUrl: './todo-list.component.css'
})
export class TodoListComponent implements OnInit {
  protected readonly service = inject(TodoService);

  // Local UI signal — only lives in this component
  protected readonly newTitle = signal('');

  ngOnInit(): void {
    this.service.loadAll().subscribe();
  }

  protected add(): void {
    const title = this.newTitle().trim();
    if (!title) return;
    this.service.add(title).subscribe();
    this.newTitle.set('');
  }
}
