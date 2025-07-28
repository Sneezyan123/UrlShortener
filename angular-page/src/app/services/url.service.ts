import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { UrlHistory, ShortenUrlRequest } from '../models/url-history.model';

@Injectable({
  providedIn: 'root'
})
export class UrlService {
  private readonly apiUrl = '/api/urls';

  constructor(private http: HttpClient) {}

  getUrlHistory(): Observable<UrlHistory[]> {
    return this.http.get<UrlHistory[]>(`${this.apiUrl}/history`);
  }

  shortenUrl(request: ShortenUrlRequest): Observable<UrlHistory> {
    return this.http.post<UrlHistory>(`${this.apiUrl}/shorten`, request);
  }
}