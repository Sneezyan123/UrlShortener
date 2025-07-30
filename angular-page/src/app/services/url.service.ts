import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { UrlHistory, ShortenUrlRequest } from '../models/url-history.model';

@Injectable({
  providedIn: 'root'
})
export class UrlService {
  private readonly apiUrl = '/api/urls';

  constructor(private http: HttpClient) {}

  getUrlHistory(): Observable<UrlHistory[]> {
    return this.http.get<any>(`${this.apiUrl}/history`)
      .pipe(
        map(response => {
          if (Array.isArray(response)) {
            return response.map(item => this.mapToUrlHistory(item));
          }
          console.warn('Unexpected response format for URL history:', response);
          return [];
        }),
        catchError(this.handleError)
      );
  }

  shortenUrl(request: ShortenUrlRequest): Observable<UrlHistory> {
    const headers = this.getRequestHeaders();
    const requestBody = { originalUrl: request.url };

    return this.http.post<any>(`${this.apiUrl}/shorten`, requestBody, { headers })
      .pipe(
        map(response => this.mapToUrlHistory(response)),
        catchError(this.handleError)
      );
  }

  private mapToUrlHistory(item: any): UrlHistory {
    return {
      id: item.id || item.shortenedUrlId || item.Id,
      originalUrl: item.originalUrl || item.OriginalUrl,
      shortCode: item.shortCode || item.ShortCode,
      shortUrl: item.shortUrl || this.buildShortUrl(item.shortCode || item.ShortCode),
      createdAt: new Date(item.createdAt || item.CreatedAt || Date.now()),
      creatorEmail: item.creatorEmail || item.CreatorEmail
    };
  }

  private buildShortUrl(shortCode: string): string {
    if (!shortCode) {
      return '';
    }
    const baseUrl = window.location.origin;
    return `${baseUrl}/s/${shortCode}`;
  }

  private getRequestHeaders(): HttpHeaders {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'X-Requested-With': 'XMLHttpRequest'
    });

    const token = this.getAntiForgeryToken();
    if (token) {
      return headers.set('RequestVerificationToken', token);
    }

    return headers;
  }

  private getAntiForgeryToken(): string {
    const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]') as HTMLInputElement;
    return tokenInput?.value || '';
  }

  private handleError(error: any): Observable<never> {
    console.error('API Error:', error);
    return throwError(() => error);
  }
}