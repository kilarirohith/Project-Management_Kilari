import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import {AuthService} from '../auth/auth.service';

export interface User {
  id: number;
  fullName: string;
  email: string;
  role?: { id: number; name: string };
}
export interface SimpleUser {
  id: number;
  username: string;
  fullName: string;
}

// src/app/services/user.service.ts

// ... imports and interface ...

@Injectable({ providedIn: 'root' })
export class UserService {
  private apiUrl = 'http://localhost:5089/api/User'; 

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

  getSimpleUsers(): Observable<SimpleUser[]> {
    return this.http.get<SimpleUser[]>(this.apiUrl, this.getOptions());
  }

  getAllUsers(): Observable<User[]> {
    return this.http.get<User[]>(this.apiUrl, this.getOptions());
  }

  // ✅ ADD THIS METHOD
  getUserById(id: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`, this.getOptions());
  }

  createUser(userData: any): Observable<any> {
    return this.http.post(this.apiUrl, userData, this.getOptions());
  }

  updateUser(id: number, userData: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, userData, this.getOptions());
  }

  deleteUser(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`, this.getOptions());
  }
}