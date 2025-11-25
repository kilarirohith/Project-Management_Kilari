// src/app/models/task.model.ts

export interface Task {
  id?: number;

  title: string;
  description: string;

  status: string;          // Open, In Progress, Closed
  priority: string;        // Low, Normal, High

  projectId: number;
  projectName?: string;

  assignedToUserId?: number | null;
  assignedUserName?: string | null;

  createdAt?: string;      // Raised Date
  dueDate?: string | null; // Expected Closure Date

  progress?: number | null; // 0–100
}

export interface CreateTaskPayload {
  title: string;
  description?: string;
  status?: string;
  priority?: string;
  projectId: number;
  assignedToUserId?: number | null;
  dueDate?: string | null;
}

export interface TaskTrackerPayload {
  taskId: number;
  progress: number;
  remarks?: string;
}
