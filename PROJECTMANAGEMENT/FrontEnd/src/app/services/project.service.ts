import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, catchError, of } from 'rxjs';
import { ProjectDTO } from '../models/project.model';
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

  createProject(dto: Partial<ProjectDTO>): Observable<ProjectDTO> {
    return this.http.post<ProjectDTO>(this.apiUrl, dto, this.getOptions());
  }

  updateProject(id: number, dto: Partial<ProjectDTO>): Observable<ProjectDTO> {
    return this.http.put<ProjectDTO>(`${this.apiUrl}/${id}`, dto, this.getOptions());
  }

  deleteProject(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`, this.getOptions());
  }
}
