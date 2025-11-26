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

  // Issue & Requirement fields
  type?: string | null;              // Issue / Requirement
  produceStep?: string | null;
  sampleData?: string | null;
  acceptanceCriteria?: string | null;
  actualClosureDate?: string | null;
  testingStatus?: string | null;
  testingDoneBy?: string | null;
}

export interface CreateTaskPayload {
  title: string;
  description?: string;
  status?: string;
  priority?: string;
  projectId: number;
  assignedToUserId?: number | null;
  dueDate?: string | null;

  type?: string | null;
  produceStep?: string | null;
  sampleData?: string | null;
  acceptanceCriteria?: string | null;
  actualClosureDate?: string | null;
  testingStatus?: string | null;
  testingDoneBy?: string | null;
}
