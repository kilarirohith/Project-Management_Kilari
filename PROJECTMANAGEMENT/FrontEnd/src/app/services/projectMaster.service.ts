import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from '../auth/auth.service';

export interface ProjectMaster {
  id?: number;
  projectName: string;
  description?: string;
  clientId: number;
  clientName?: string; // Read-only from backend for the table
}

@Injectable({
  providedIn: 'root'
})
export class ProjectMasterService {
  // Matches your C# Controller Route: api/ProjectMaster
  private apiUrl = 'http://localhost:5089/api/ProjectMaster'; 

  constructor(private http: HttpClient, private authService: AuthService) {}

  private getOptions() {
    const token = this.authService.getToken();
    return {
      headers: new HttpHeaders({
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      })
    };
  }

  getAllProjects(): Observable<ProjectMaster[]> {
    return this.http.get<ProjectMaster[]>(this.apiUrl, this.getOptions());
  }

  getProjectById(id: number): Observable<ProjectMaster> {
    return this.http.get<ProjectMaster>(`${this.apiUrl}/${id}`, this.getOptions());
  }

  createProject(project: ProjectMaster): Observable<ProjectMaster> {
    return this.http.post<ProjectMaster>(this.apiUrl, project, this.getOptions());
  }

  updateProject(id: number, project: ProjectMaster): Observable<ProjectMaster> {
    return this.http.put<ProjectMaster>(`${this.apiUrl}/${id}`, project, this.getOptions());
  }

  deleteProject(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`, this.getOptions());
  }
}