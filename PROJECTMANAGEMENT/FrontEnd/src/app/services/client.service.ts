import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from '../auth/auth.service';

export interface Unit {
  id?: number;
  unitName: string;
}

export interface Location {
  id?: number;
  locationName: string;
  spoc?: string;
  units: Unit[];
}

export interface Client {
  id?: number;
  clientName: string;
  gst: string;
  email: string;
  locations: Location[];
}

@Injectable({ providedIn: 'root' })
export class ClientService {
  private apiUrl = 'http://localhost:5089/api/Client';

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

  getAllClients(): Observable<Client[]> {
    return this.http.get<Client[]>(this.apiUrl, this.getOptions());
  }

  getClientById(id: number): Observable<Client> {
    return this.http.get<Client>(`${this.apiUrl}/${id}`, this.getOptions());
  }

  // For "Location *" dropdown when a client is selected
  getLocationsByClient(clientId: number): Observable<Location[]> {
    return this.http.get<Location[]>(`${this.apiUrl}/${clientId}/locations`, this.getOptions());
  }

  createClient(client: Client): Observable<Client> {
    return this.http.post<Client>(this.apiUrl, client, this.getOptions());
  }

  updateClient(id: number, client: Client): Observable<Client> {
    return this.http.put<Client>(`${this.apiUrl}/${id}`, client, this.getOptions());
  }

  deleteClient(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`, this.getOptions());
  }
}
