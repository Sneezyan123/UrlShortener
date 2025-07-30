import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subject, takeUntil, timer } from 'rxjs';
import { UrlService } from '../../services/url.service';
import { AuthService, UserInfo } from '../../services/auth.service';
import { NavigationService } from '../../services/navigation.service';
import { ErrorHandlerService } from '../../services/error-handler.service';
import { UrlHistory } from '../../models/url-history.model';

@Component({
  selector: 'app-url-shortener',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './url-shortener.component.html',
  styleUrls: ['./url-shortener.component.scss']
})
export class UrlShortenerComponent implements OnInit, OnDestroy {
  urlToShorten: string = '';
  shortenedUrls: UrlHistory[] = [];
  isLoading: boolean = false; // Починаємо з false
  errorMessage: string = '';
  successMessage: string = '';
  currentUser: UserInfo = { isAuthenticated: false };
  
  private destroy$ = new Subject<void>();

  constructor(
    private urlService: UrlService,
    private authService: AuthService,
    private navigationService: NavigationService,
    private errorHandler: ErrorHandlerService,
    private cdr: ChangeDetectorRef
  ) {
    // Ініціалізуємо стан при створенні компонента
    console.log('Component constructor called');
  }

  ngOnInit(): void {
    console.log('Component initialized'); // Для дебагу
    
    // Підписуємося на зміни стану авторизації
    this.authService.user$
      .pipe(takeUntil(this.destroy$))
      .subscribe(user => {
        console.log('User state changed:', user); // Для дебагу
        this.currentUser = user;
        
        // Скидаємо стан завантаження при зміні користувача
        this.isLoading = false;
        
        // Форсуємо оновлення UI
        this.triggerChangeDetection();
        
        if (user.isAuthenticated) {
          this.loadUrlHistory();
        } else {
          this.shortenedUrls = [];
        }
      });
    
    // Спочатку перевіряємо авторизацію з DOM
    this.authService.checkAuthFromDOM();
    
    // Через деякий час робимо серверну перевірку для надійності
    timer(1000).pipe(takeUntil(this.destroy$)).subscribe(() => {
      this.authService.checkAuthStatus().subscribe();
    });
  }

  private triggerChangeDetection(): void {
    // Форсуємо Angular перевірити зміни
    this.cdr.detectChanges();
    setTimeout(() => {
      console.log('Change detection triggered, current user:', this.currentUser);
      this.cdr.markForCheck();
    }, 0);
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadUrlHistory(): void {
    // Не блокуємо весь UI при завантаженні історії
    console.log('Loading URL history...');
    
    this.urlService.getUrlHistory().subscribe({
      next: (history) => {
        this.shortenedUrls = history.map(url => ({
          ...url,
          shortUrl: this.buildShortUrl(url.shortCode || url.id.toString()),
          createdAt: new Date(url.createdAt)
        }));
        console.log('Loaded URL history:', this.shortenedUrls); // Для дебагу
      },
      error: (error) => {
        console.error('Error loading URL history:', error);
        // Не показуємо помилку користувачу для історії, просто логуємо
        this.shortenedUrls = [];
      }
    });
  }

  canShortenUrl(): boolean {
    const can = !this.isLoading && 
                this.urlToShorten.trim().length > 0 && 
                this.currentUser.isAuthenticated === true;
    
    // Логуємо тільки коли стан змінюється
    const newDebugState = {
      isLoading: this.isLoading,
      hasUrl: this.urlToShorten.trim().length > 0,
      isAuth: this.currentUser.isAuthenticated,
      result: can
    };
    
    // Зберігаємо попередній стан для порівняння
    if (!this.lastDebugState || JSON.stringify(this.lastDebugState) !== JSON.stringify(newDebugState)) {
      console.log('Can shorten URL changed:', newDebugState);
      this.lastDebugState = newDebugState;
    }
    
    return can;
  }

  private lastDebugState: any = null;

  getButtonTitle(): string {
    if (!this.currentUser.isAuthenticated) {
      return 'Please sign in to shorten URLs';
    }
    if (!this.urlToShorten.trim()) {
      return 'Enter a URL to shorten';
    }
    if (this.isLoading) {
      return 'Processing...';
    }
    return 'Click to shorten URL';
  }

  // Додаємо метод для дебагу
  debugCurrentUser(): void {
    console.log('Debug - current user state:', {
      isAuthenticated: this.currentUser.isAuthenticated,
      userName: this.currentUser.userName,
      email: this.currentUser.email,
      canShortenUrl: this.currentUser.isAuthenticated && !this.isLoading && this.urlToShorten.trim()
    });
  }

  shortenUrl(): void {
    // Валідація вводу
    if (!this.urlToShorten.trim()) {
      this.showError('Please enter a valid URL');
      return;
    }

    if (!this.isValidUrl(this.urlToShorten)) {
      this.showError('Please enter a valid URL format (e.g., https://example.com)');
      return;
    }

    if (!this.currentUser.isAuthenticated) {
      this.showError('You must be signed in to shorten URLs');
      return;
    }

    this.isLoading = true;
    this.clearMessages();

    this.urlService.shortenUrl({ url: this.urlToShorten }).subscribe({
      next: (response) => {
        console.log('URL shortened successfully:', response); // Для дебагу
        
        const newUrl: UrlHistory = {
          ...response,
          shortUrl: this.buildShortUrl(response.shortCode || response.id.toString()),
          createdAt: new Date(response.createdAt)
        };
        
        this.shortenedUrls = [newUrl, ...this.shortenedUrls];
        this.urlToShorten = '';
        this.showSuccess('URL shortened successfully!');
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error shortening URL:', error);
        this.errorHandler.handleError(error, 'shortenUrl');
        
        if (error.status === 401) {
          this.showError('You must be signed in to shorten URLs');
          // Можливо, токен експайрувався, перевіряємо авторизацію знову
          this.authService.checkAuthStatus().subscribe();
        } else {
          this.showError(this.errorHandler.getUserFriendlyErrorMessage(error));
        }
        
        this.isLoading = false;
      }
    });
  }

  navigateToHome(): void {
    window.location.href = '/';
  }

  navigateToAbout(): void {
    window.location.href = '/home/about';
  }

  onSignIn(): void {
    this.navigationService.navigateToLogin();
  }

  onSignUp(): void {
    this.navigationService.navigateToRegister();
  }

  onSignOut(): void {
    this.authService.logout().subscribe({
      next: () => {
        // Успішний logout через API
        this.clearMessages();
        this.showSuccess('Successfully logged out');
        this.shortenedUrls = []; // Очищаємо історію
      },
      error: (error) => {
        console.error('Logout error:', error);
        // Навіть якщо API не спрацював, пробуємо стандартний спосіб
        window.location.href = '/user/logout';
      }
    });
  }

  clearError(): void {
    this.errorMessage = '';
  }

  clearSuccess(): void {
    this.successMessage = '';
  }

  private clearMessages(): void {
    this.errorMessage = '';
    this.successMessage = '';
  }

  private showError(message: string): void {
    this.errorMessage = message;
    this.successMessage = '';
  }

  private showSuccess(message: string): void {
    this.successMessage = message;
    this.errorMessage = '';
    
    // Автоматично приховуємо повідомлення успіху через 3 секунди
    timer(3000).pipe(takeUntil(this.destroy$)).subscribe(() => {
      this.successMessage = '';
    });
  }

  isValidUrl(url: string): boolean {
    try {
      const urlObj = new URL(url);
      return urlObj.protocol === 'http:' || urlObj.protocol === 'https:';
    } catch {
      try {
        // Спробуємо додати https:// якщо протокол не вказаний
        new URL('https://' + url);
        this.urlToShorten = 'https://' + url;
        return true;
      } catch {
        return false;
      }
    }
  }

  copyToClipboard(url: string): void {
    navigator.clipboard.writeText(url).then(() => {
      this.showSuccess('URL copied to clipboard!');
    }).catch(() => {
      // Fallback для старих браузерів
      const textArea = document.createElement('textarea');
      textArea.value = url;
      document.body.appendChild(textArea);
      textArea.select();
      document.execCommand('copy');
      document.body.removeChild(textArea);
      
      this.showSuccess('URL copied to clipboard!');
    });
  }

  private buildShortUrl(shortCode: string): string {
    const baseUrl = window.location.origin;
    return `${baseUrl}/s/${shortCode}`;
  }
}