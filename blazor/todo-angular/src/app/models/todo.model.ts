export interface Todo {
  id: number;
  title: string;
  isComplete: boolean;
  createdAt: string;
}

export interface CreateTodo {
  title: string;
  isComplete: boolean;
}
