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

  // Виправлений endpoint - тепер використовуємо правильний URL
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

  // Виправлений logout з правильним endpoint та CSRF токеном
  logout(): Observable<any> {
    // Отримуємо CSRF токен
    const token = this.getAntiForgeryToken();
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'RequestVerificationToken': token
    });

    return this.http.post('/user/logout', {}, { headers }).pipe(
      tap(() => {
        this.userSubject.next({ isAuthenticated: false });
      }),
      catchError((error) => {
        console.error('Logout failed:', error);
        // Навіть якщо logout не вдався, вважаємо користувача вийшовшим
        this.userSubject.next({ isAuthenticated: false });
        return of({ success: false });
      })
    );
  }

  // Покращена функція для перевірки авторизації з DOM
  checkAuthFromDOM(): void {
    console.log('Checking auth from DOM...');
    
    // Спочатку спробуємо DOM методи (вони швидші)
    const foundAuth = this.tryExtractAuthFromDOM();
    
    if (foundAuth) {
      console.log('Auth found in DOM, skipping server check for now');
    } else {
      console.log('No auth found in DOM, trying server...');
      // Якщо в DOM нічого не знайшли, спробуємо сервер
      this.checkAuthStatus().subscribe({
        next: (userInfo) => {
          console.log('Server auth check result:', userInfo);
        },
        error: (error) => {
          console.log('Server auth check failed:', error);
          // Встановлюємо неавторизований стан
          this.userSubject.next({ isAuthenticated: false });
        }
      });
    }
  }

  private tryExtractAuthFromDOM(): boolean {
    console.log('Trying to extract auth info from DOM...');
    
    // Перевіряємо різні способи визначення авторизації в DOM
    const methods = [
      () => this.checkUserMetaTags(),
      () => this.checkAuthElements(),
      () => this.checkNavbarUserInfo(),
      () => this.checkAuthCookies()
    ];

    for (const method of methods) {
      if (method()) {
        console.log('Auth found using method:', method.name);
        return true; // Якщо один метод спрацював, зупиняємося
      }
    }
    
    console.log('No auth info found in DOM');
    return false;
  }

  private checkNavbarUserInfo(): boolean {
    console.log('Checking navbar for user info...');
    
    // Шукаємо в навбарі інформацію про користувача
    const navbarTexts = document.querySelectorAll('.navbar-text, .welcome-text');
    console.log('Found navbar texts:', navbarTexts.length);
    
    for (const element of navbarTexts) {
      const text = element.textContent || '';
      console.log('Checking navbar text:', text);
      
      if (text.includes('Welcome') || text.includes('Ласкаво просимо')) {
        const userName = this.extractUserNameFromText(text);
        console.log('Extracted username from navbar:', userName);
        
        if (userName) {
          const userInfo = {
            isAuthenticated: true,
            userName: userName,
            email: userName // Якщо email не знайдено, використовуємо userName
          };
          console.log('Setting user info from navbar:', userInfo);
          this.userSubject.next(userInfo);
          return true;
        }
      }
    }
    return false;
  }

  private checkUserMetaTags(): boolean {
    console.log('Checking meta tags for user info...');
    
    // Перевіряємо meta теги з інформацією про користувача
    const userIdMeta = document.querySelector('meta[name="user-id"]') as HTMLMetaElement;
    const userEmailMeta = document.querySelector('meta[name="user-email"]') as HTMLMetaElement;
    const isAuthMeta = document.querySelector('meta[name="is-authenticated"]') as HTMLMetaElement;

    console.log('Meta tags found:', {
      userId: userIdMeta?.content,
      userEmail: userEmailMeta?.content,
      isAuth: isAuthMeta?.content
    });

    if (isAuthMeta && isAuthMeta.content === 'true') {
      const userInfo = {
        isAuthenticated: true,
        userId: userIdMeta?.content,
        email: userEmailMeta?.content,
        userName: userEmailMeta?.content
      };
      console.log('Setting user info from meta tags:', userInfo);
      this.userSubject.next(userInfo);
      return true;
    }
    return false;
  }

  private checkAuthElements(): boolean {
    console.log('Checking auth elements...');
    
    // Перевіряємо елементи з data-атрибутами
    const authElements = document.querySelectorAll('[data-user-authenticated="true"]');
    console.log('Found auth elements:', authElements.length);
    
    if (authElements.length > 0) {
      const authElement = authElements[0] as HTMLElement;
      const userName = authElement.dataset['userEmail'] || authElement.dataset['userName'];
      
      console.log('Auth element data:', {
        userEmail: authElement.dataset['userEmail'],
        userName: authElement.dataset['userName'],
        userId: authElement.dataset['userId']
      });
      
      if (userName) {
        const userInfo = {
          isAuthenticated: true,
          userName: userName,
          email: authElement.dataset['userEmail'],
          userId: authElement.dataset['userId']
        };
        console.log('Setting user info from auth elements:', userInfo);
        this.userSubject.next(userInfo);
        return true;
      }
    }
    return false;
  }

  private checkAuthCookies(): boolean {
    // Перевіряємо наявність cookies авторизації
    if (this.hasAuthCookie()) {
      // Якщо є cookie, робимо серверний запит для отримання деталей
      this.checkAuthStatus().subscribe();
      return true;
    }
    return false;
  }

  private extractUserNameFromText(text: string): string {
    // Покращене регулярне вираження для витягання імені користувача
    const patterns = [
      /(?:Welcome,?\s*|Ласкаво просимо,?\s*|Hello\s*)(.*?)(?:\s*!|$)/i,
      /(?:Вітаємо,?\s*)(.*?)(?:\s*!|$)/i
    ];

    for (const pattern of patterns) {
      const matches = text.match(pattern);
      if (matches && matches[1]) {
        return matches[1].trim();
      }
    }
    return '';
  }

  private hasAuthCookie(): boolean {
    const authCookieNames = [
      'CookieAuth',
      '.AspNetCore.Identity.Application', 
      'auth-token', 
      'session',
      '.AspNetCore.Cookies'
    ];
    return authCookieNames.some(name => 
      document.cookie.split(';').some(cookie => 
        cookie.trim().startsWith(name + '=')
      )
    );
  }

  private getAntiForgeryToken(): string {
    const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]') as HTMLInputElement;
    if (tokenInput) {
      return tokenInput.value;
    }

    const tokenMeta = document.querySelector('meta[name="__RequestVerificationToken"]') as HTMLMetaElement;
    if (tokenMeta) {
      return tokenMeta.content;
    }

    const tokenCookie = document.cookie
      .split(';')
      .find(cookie => cookie.trim().startsWith('__RequestVerificationToken='));
    
    if (tokenCookie) {
      return tokenCookie.split('=')[1];
    }

    return '';
  }
}