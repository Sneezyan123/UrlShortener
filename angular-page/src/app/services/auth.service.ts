import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { map, catchError } from 'rxjs/operators';

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
    return this.http.get<any>('/api/auth/status').pipe(
      map(response => {
        const userInfo: UserInfo = {
          isAuthenticated: response.isAuthenticated,
          userName: response.userName,
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
        return [userInfo];
      })
    );
  }

  logout(): Observable<any> {
    return this.http.post('/api/auth/logout', {}).pipe(
      map(() => {
        this.userSubject.next({ isAuthenticated: false });
        return { success: true };
      }),
      catchError((error) => {
        console.error('Logout failed:', error);
        this.userSubject.next({ isAuthenticated: false });
        return [{ success: false }];
      })
    );
  }

  checkAuthFromDOM(): void {
    const authElements = [
      document.querySelector('[data-user-authenticated="true"]'),
      document.querySelector('.user-info'),
      document.querySelector('.navbar-text')
    ];

    const authElement = authElements.find(el => el !== null) as HTMLElement;
    
    if (authElement) {
      const userName = authElement.dataset["userEmail"] || 
                     this.extractUserNameFromText(authElement.textContent || '');
      
      if (userName) {
        this.userSubject.next({
          isAuthenticated: true,
          email: authElement.dataset["userEmail"],
        });
        return;
      }
    }

    if (this.hasAuthCookie()) {
      this.checkAuthStatus().subscribe();
    } else {
      this.userSubject.next({ isAuthenticated: false });
    }
  }

  private extractUserNameFromText(text: string): string {
    const matches = text.match(/(?:Welcome,?\s*|Hello\s*)(.*?)(?:\s*!|$)/i);
    return matches ? matches[1].trim() : '';
  }

  private hasAuthCookie(): boolean {
    const authCookieNames = ['.AspNetCore.Identity.Application', 'auth-token', 'session'];
    return authCookieNames.some(name => document.cookie.includes(name));
  }
}