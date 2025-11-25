import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface TaskTracker {
  id?: number;
  taskId: number; // The link to the Task
  progress: number;
  startTime?: string;
  endTime?: string;
  remarks?: string;
  updatedAt?: string;
  task?: { title: string }; // Optional: To display the Task Name in the table
}

@Injectable({ providedIn: 'root' })
export class TaskTrackerService {
  private apiUrl = 'http://localhost:5089/api/TaskTracker';

  constructor(private http: HttpClient) {}

  getAll(): Observable<TaskTracker[]> {
    return this.http.get<TaskTracker[]>(this.apiUrl);
  }

  create(data: TaskTracker): Observable<any> {
    return this.http.post(this.apiUrl, data);
  }

  delete(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}