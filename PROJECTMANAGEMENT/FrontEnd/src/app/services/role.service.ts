import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http'; // ✅ Import HttpHeaders
import { Observable } from 'rxjs';
import { AuthService } from '../auth/auth.service'; // ✅ Import AuthService

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

@Injectable({ providedIn: 'root' })
export class RoleService {
  private apiUrl = 'http://localhost:5089/api/Role'; // Check your port

  constructor(
    private http: HttpClient,
    private authService: AuthService // ✅ Inject Auth Service
  ) {}

  // ✅ Helper: Get Token from Auth Service and create Headers
  private getOptions() {
    const token = this.authService.getToken();
    return {
      headers: new HttpHeaders({
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      })
    };
  }

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
}