// src/app/services/role.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from '../auth/auth.service';

export interface RolePermission {
  module: string;
  canCreate: boolean;
  canRead: boolean;
  canUpdate: boolean;
  canDelete: boolean;
}

export interface Role {
  id?: number;
  name: string;
  description?: string;
  permissions: RolePermission[];
}

export interface AppModule {
  id?: number;
  name: string;
}

@Injectable({ providedIn: 'root' })
export class RoleService {
  private apiUrl = 'http://localhost:5089/api/Role';
  private moduleApi = 'http://localhost:5089/api/Module';

  constructor(
    private http: HttpClient,
    private authService: AuthService
  ) {}

  private getOptions() {
    const token = this.authService.getToken();
    return {
      headers: new HttpHeaders({
        Authorization: `Bearer ${token}`,
        'Content-Type': 'application/json'
      })
    };
  }

  // ---- Roles ----
  getRoles(): Observable<Role[]> {
    return this.http.get<Role[]>(this.apiUrl, this.getOptions());
  }

  createRole(role: Role): Observable<Role> {
    return this.http.post<Role>(this.apiUrl, role, this.getOptions());
  }

  updateRole(id: number, role: Role): Observable<Role> {
    return this.http.put<Role>(`${this.apiUrl}/${id}`, role, this.getOptions());
  }

  deleteRole(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`, this.getOptions());
  }

  // ---- Modules ----
  getModules(): Observable<AppModule[]> {
    return this.http.get<AppModule[]>(this.moduleApi, this.getOptions());
  }

  createModule(name: string): Observable<AppModule> {
    return this.http.post<AppModule>(this.moduleApi, { name }, this.getOptions());
  }

  deleteModule(id: number): Observable<any> {
    return this.http.delete(`${this.moduleApi}/${id}`, this.getOptions());
  }
}
