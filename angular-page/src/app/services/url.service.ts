import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { UrlHistory, ShortenUrlRequest } from '../models/url-history.model';

@Injectable({
  providedIn: 'root'
})
export class UrlService {
  private readonly apiUrl = '/api/urls';

  constructor(private http: HttpClient) {}

  getUrlHistory(): Observable<UrlHistory[]> {
    return this.http.get<UrlHistory[]>(`${this.apiUrl}/history`)
      .pipe(
        catchError(this.handleError)
      );
  }

  shortenUrl(request: ShortenUrlRequest): Observable<UrlHistory> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json'
    });

    return this.http.post<UrlHistory>(`${this.apiUrl}/shorten`, {
      originalUrl: request.url
    }, { headers })
      .pipe(
        catchError(this.handleError)
      );
  }

  private handleError(error: any): Observable<never> {
    console.error('API Error:', error);
    return throwError(() => error);
  }
}