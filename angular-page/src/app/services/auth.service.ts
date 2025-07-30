import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, BehaviorSubject, of } from 'rxjs';
import { map, catchError, tap } from 'rxjs/operators';

export interface UserInfo {
  isAuthenticated: boolean;
  userName?: string;
  email?: string;
  userId?: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private userSubject = new BehaviorSubject<UserInfo>({ isAuthenticated: false });
  public user$ = this.userSubject.asObservable();

  constructor(private http: HttpClient) {}

  get isAuthenticated(): boolean {
    return this.userSubject.value.isAuthenticated;
  }

  get currentUser(): UserInfo {
    return this.userSubject.value;
  }

  checkAuthStatus(): Observable<UserInfo> {
    return this.http.get<any>('/user/status').pipe(
      map(response => {
        const userInfo: UserInfo = {
          isAuthenticated: response.isAuthenticated,
          userName: response.userName || response.email,
          email: response.email,
          userId: response.userId
        };
        this.userSubject.next(userInfo);
        return userInfo;
      }),
      catchError((error) => {
        console.error('Auth check failed:', error);
        const userInfo: UserInfo = { isAuthenticated: false };
        this.userSubject.next(userInfo);
        return of(userInfo);
      })
    );
  }

  logout(): Observable<any> {
    const token = this.getAntiForgeryToken();
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'RequestVerificationToken': token,
      'X-Requested-With': 'XMLHttpRequest'
    });

    return this.http.post('/user/logout', {}, { headers }).pipe(
      tap(() => {
        this.userSubject.next({ isAuthenticated: false });
      }),
      catchError((error) => {
        console.error('Logout failed:', error);
        this.userSubject.next({ isAuthenticated: false });
        return of({ success: false });
      })
    );
  }

  checkAuthFromDOM(): void {
    this.checkAuthStatus().subscribe();
  }

  private getAntiForgeryToken(): string {
    const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]') as HTMLInputElement;
    return tokenInput?.value || '';
  }
}