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
          // Обробляємо різні формати відповіді від сервера
          if (Array.isArray(response)) {
            return response.map(item => this.mapToUrlHistory(item));
          }
          // Якщо відповідь не масив, повертаємо порожній масив
          console.warn('Unexpected response format for URL history:', response);
          return [];
        }),
        catchError(this.handleError)
      );
  }

  shortenUrl(request: ShortenUrlRequest): Observable<UrlHistory> {
    const headers = this.getRequestHeaders();

    const requestBody = {
      originalUrl: request.url
    };

    return this.http.post<any>(`${this.apiUrl}/shorten`, requestBody, { headers })
      .pipe(
        map(response => this.mapToUrlHistory(response)),
        catchError(this.handleError)
      );
  }

  private mapToUrlHistory(item: any): UrlHistory {
    // Мапимо відповідь сервера до нашої моделі
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
    let headers = new HttpHeaders({
      'Content-Type': 'application/json'
    });

    // Додаємо CSRF токен
    const token = this.getAntiForgeryToken();
    if (token) {
      headers = headers.set('RequestVerificationToken', token);
      headers = headers.set('X-CSRF-TOKEN', token);
    }

    // Вказуємо, що це AJAX запит
    headers = headers.set('X-Requested-With', 'XMLHttpRequest');

    return headers;
  }

  private getAntiForgeryToken(): string {
    // Спробуємо отримати токен з різних джерел
    const sources = [
      () => {
        const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]') as HTMLInputElement;
        return tokenInput?.value;
      },
      () => {
        const tokenMeta = document.querySelector('meta[name="__RequestVerificationToken"]') as HTMLMetaElement;
        return tokenMeta?.content;
      },
      () => {
        const tokenCookie = document.cookie
          .split(';')
          .find(cookie => cookie.trim().startsWith('__RequestVerificationToken='));
        return tokenCookie?.split('=')[1];
      },
      () => {
        // Спробуємо отримати з заголовка відповіді (якщо сервер його встановлює)
        return (window as any).__CSRF_TOKEN__;
      }
    ];

    for (const getToken of sources) {
      const token = getToken();
      if (token) {
        return token;
      }
    }

    console.warn('CSRF token not found');
    return '';
  }

  private handleError(error: any): Observable<never> {
    console.error('API Error:', error);
    
    // Додаткова інформація для дебагу
    if (error.status === 400) {
      console.error('Bad Request - possibly invalid data format');
    } else if (error.status === 401) {
      console.error('Unauthorized - user needs to login');
    } else if (error.status === 403) {
      console.error('Forbidden - possibly CSRF token issue');
    }
    
    return throwError(() => error);
  }
}