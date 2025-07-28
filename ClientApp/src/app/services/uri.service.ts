import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UriService {
  private baseUrl = 'api/uri';

  constructor(private http: HttpClient) { }

  isAuthenticated(): boolean {
    const token = localStorage.getItem('jwt_token');
    return !!token;
  }

  getAuthHeaders(): HttpHeaders {
    const token = localStorage.getItem('jwt_token');
    return new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`
    });
  }

  shortenUrl(longUrl: string): Observable<any> {
    if (!this.isAuthenticated()) {
      throw new Error('User must be authenticated to shorten URLs');
    }

    const headers = this.getAuthHeaders();
    return this.http.post(this.baseUrl, { longUrl }, { headers });
  }
}
