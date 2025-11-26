// src/app/services/ticket.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Ticket } from '../models/ticket.model';
import { AuthService } from '../auth/auth.service';

@Injectable({ providedIn: 'root' })
export class TicketService {
  private apiUrl = 'http://localhost:5089/api/Ticket';

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

  getAll(): Observable<Ticket[]> {
    return this.http.get<Ticket[]>(this.apiUrl, this.getOptions());
  }

  getById(id: number): Observable<Ticket> {
    return this.http.get<Ticket>(`${this.apiUrl}/${id}`, this.getOptions());
  }

  create(ticket: any): Observable<Ticket> {
    return this.http.post<Ticket>(this.apiUrl, ticket, this.getOptions());
  }

  update(id: number, ticket: any): Observable<Ticket> {
    return this.http.put<Ticket>(`${this.apiUrl}/${id}`, ticket, this.getOptions());
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`, this.getOptions());
  }
}
