import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from '../auth/auth.service';

export interface VendorWork {
  id?: number;
  projectName: string;
  workDescription?: string;
  startDate: string; // 'yyyy-MM-dd' for form, ISO string from backend
  endDate: string;
  status: string;
  vendorId: number;
  vendorName?: string; // From backend
}

@Injectable({ providedIn: 'root' })
export class VendorWorkService {
  private apiUrl = 'http://localhost:5089/api/VendorWork';

  constructor(private http: HttpClient, private authService: AuthService) {}

  private getOptions() {
    const token = this.authService.getToken();
    return {
      headers: new HttpHeaders({
        Authorization: `Bearer ${token}`,
        'Content-Type': 'application/json'
      })
    };
  }

  getAll(): Observable<VendorWork[]> {
    return this.http.get<VendorWork[]>(this.apiUrl, this.getOptions());
  }

  create(data: Partial<VendorWork>): Observable<VendorWork> {
    return this.http.post<VendorWork>(this.apiUrl, data, this.getOptions());
  }

  update(id: number, data: Partial<VendorWork>): Observable<VendorWork> {
    return this.http.put<VendorWork>(`${this.apiUrl}/${id}`, data, this.getOptions());
  }

  delete(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`, this.getOptions());
  }
}
