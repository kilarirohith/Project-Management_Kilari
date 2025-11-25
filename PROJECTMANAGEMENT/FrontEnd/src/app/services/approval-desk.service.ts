import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from '../auth/auth.service';

export interface ApprovalDesk {
  id?: number;
  status: string;      // Pending, Approved, Rejected, On Hold
  remarks: string;

  projectId: number;
  project?: { projectName: string };

  vendorWorkId: number;
  vendorWork?: {
    projectName: string;
    vendor?: { vendorName: string };
  };
}

@Injectable({
  providedIn: 'root'
})
export class ApprovalDeskService {
  private apiUrl = 'http://localhost:5089/api/ApprovalDesk';

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

  // GET /api/ApprovalDesk
  getAll(): Observable<ApprovalDesk[]> {
    return this.http.get<ApprovalDesk[]>(this.apiUrl, this.getOptions());
  }

  // POST /api/ApprovalDesk
  create(data: {
    projectId: number;
    vendorWorkId: number;
    status: string;
    remarks: string;
  }): Observable<ApprovalDesk> {
    return this.http.post<ApprovalDesk>(this.apiUrl, data, this.getOptions());
  }

  // PUT /api/ApprovalDesk/{id}
  update(
    id: number,
    data: {
      projectId: number;
      vendorWorkId: number;
      status: string;
      remarks: string;
    }
  ): Observable<ApprovalDesk> {
    return this.http.put<ApprovalDesk>(
      `${this.apiUrl}/${id}`,
      data,
      this.getOptions()
    );
  }

  // DELETE /api/ApprovalDesk/{id}
  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`, this.getOptions());
  }
}
