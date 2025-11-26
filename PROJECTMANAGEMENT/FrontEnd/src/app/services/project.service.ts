// src/app/services/project.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, catchError, of } from 'rxjs';
import {
  ProjectDTO,
  CreateProjectPayload,
  UpdateProjectPayload
} from '../models/project.model';
import { AuthService } from '../auth/auth.service';

@Injectable({
  providedIn: 'root'
})
export class ProjectService {
  private apiUrl = 'http://localhost:5089/api/Project';

  constructor(
    private http: HttpClient,
    private authService: AuthService
  ) {}

  private getOptions() {
    const token = this.authService.getToken();
    return {
      headers: new HttpHeaders({
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      })
    };
  }

  getProjects(): Observable<ProjectDTO[]> {
    return this.http.get<ProjectDTO[]>(this.apiUrl, this.getOptions())
      .pipe(
        catchError(err => {
          console.error('Error loading projects', err);
          return of([]);
        })
      );
  }

  getProjectById(id: number): Observable<ProjectDTO> {
    return this.http.get<ProjectDTO>(`${this.apiUrl}/${id}`, this.getOptions());
  }

  // 👇 use CreateProjectPayload
  createProject(payload: CreateProjectPayload): Observable<ProjectDTO> {
    return this.http.post<ProjectDTO>(this.apiUrl, payload, this.getOptions());
  }

  // 👇 use UpdateProjectPayload
  updateProject(id: number, payload: UpdateProjectPayload): Observable<ProjectDTO> {
    return this.http.put<ProjectDTO>(`${this.apiUrl}/${id}`, payload, this.getOptions());
  }

  deleteProject(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`, this.getOptions());
  }
}
