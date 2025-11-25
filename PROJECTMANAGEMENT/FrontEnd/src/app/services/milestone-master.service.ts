import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from '../auth/auth.service';

export interface Milestone {
  id: number; // Changed from string to number to match DB
  name: string;
  weightage: number;
}

@Injectable({
  providedIn: 'root'
})
export class MilestoneMasterService {
  private apiUrl = 'http://localhost:5089/api/MilestoneMaster'; // Check your port

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

  getAll(): Observable<Milestone[]> {
    return this.http.get<Milestone[]>(this.apiUrl, this.getOptions());
  }

  create(data: any): Observable<Milestone> {
    return this.http.post<Milestone>(this.apiUrl, data, this.getOptions());
  }

  update(id: number, data: Milestone): Observable<Milestone> {
    return this.http.put<Milestone>(`${this.apiUrl}/${id}`, data, this.getOptions());
  }

  delete(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`, this.getOptions());
  }
}