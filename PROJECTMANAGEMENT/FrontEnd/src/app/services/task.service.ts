// src/app/services/task.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Task, CreateTaskPayload, TaskTrackerPayload } from '../models/task.model';
import { AuthService } from '../auth/auth.service';

@Injectable({
  providedIn: 'root'
})
export class TaskService {
  private apiUrl = 'http://localhost:5089/api/Task';
  private trackerUrl = 'http://localhost:5089/api/TaskTracker';

  constructor(
    private http: HttpClient,
    private authService: AuthService
  ) { }

  private getOptions() {
    const token = this.authService.getToken();
    return {
      headers: new HttpHeaders({
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      })
    };
  }

  getTasks(): Observable<Task[]> {
    return this.http.get<Task[]>(this.apiUrl, this.getOptions());
  }

  createTask(payload: CreateTaskPayload): Observable<any> {
    return this.http.post<any>(this.apiUrl, payload, this.getOptions());
  }

  updateTask(id: number, payload: CreateTaskPayload): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${id}`, payload, this.getOptions());
  }

  deleteTask(id: number): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${id}`, this.getOptions());
  }

  // Update / create progress (TaskTracker)
  updateProgress(payload: TaskTrackerPayload): Observable<any> {
    return this.http.post<any>(this.trackerUrl, payload, this.getOptions());
  }
}
