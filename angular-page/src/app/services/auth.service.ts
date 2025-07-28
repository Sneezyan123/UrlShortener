import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { map, catchError } from 'rxjs/operators';

export interface UserInfo {
  isAuthenticated: boolean;
  userName?: string;
  email?: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private userSubject = new BehaviorSubject<UserInfo>({ isAuthenticated: false });
  public user$ = this.userSubject.asObservable();

  constructor(private http: HttpClient) {
    this.checkAuthStatus();
  }

  get isAuthenticated(): boolean {
    return this.userSubject.value.isAuthenticated;
  }

  get currentUser(): UserInfo {
    return this.userSubject.value;
  }

  checkAuthStatus(): void {
    // Check if user is authenticated by making a request to a protected endpoint
    // or by checking for authentication cookies/tokens
    this.http.get<any>('/api/auth/status', { observe: 'response' })
      .pipe(
        map(response => {
          if (response.status === 200 && response.body) {
            return {
              isAuthenticated: true,
              userName: response.body.userName,
              email: response.body.email
            };
          }
          return { isAuthenticated: false };
        }),
        catchError(() => {
          // If the request fails, user is not authenticated
          return [{ isAuthenticated: false }];
        })
      )
      .subscribe(userInfo => {
        this.userSubject.next(userInfo);
      });
  }

  // Alternative method: check authentication based on DOM or other client-side indicators
  checkAuthFromDOM(): void {
    // This is a fallback method if you can't create an API endpoint
    // You can check if there are authentication indicators in the DOM
    const authIndicator = document.querySelector('.navbar-text') as HTMLElement;
    const isAuth = authIndicator && authIndicator.textContent?.includes('Welcome');
    
    if (isAuth) {
      const userName = authIndicator.textContent?.replace('Welcome, ', '').replace('!', '') || '';
      this.userSubject.next({
        isAuthenticated: true,
        userName: userName
      });
    } else {
      this.userSubject.next({ isAuthenticated: false });
    }
  }

  logout(): void {
    this.userSubject.next({ isAuthenticated: false });
  }
}