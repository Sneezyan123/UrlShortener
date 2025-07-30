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
  urlToShorten = '';
  shortenedUrls: UrlHistory[] = [];
  isLoading = false;
  isLoadingHistory = false;
  errorMessage = '';
  successMessage = '';
  currentUser: UserInfo = { isAuthenticated: false };
  
  private destroy$ = new Subject<void>();

  constructor(
    private urlService: UrlService,
    private authService: AuthService,
    private navigationService: NavigationService,
    private errorHandler: ErrorHandlerService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.authService.user$
      .pipe(takeUntil(this.destroy$))
      .subscribe(user => {
        this.currentUser = user;
        this.isLoading = false;
        this.cdr.detectChanges();
        
        // Завантажуємо історію для всіх користувачів
        this.loadUrlHistory();
      });
    
    this.authService.checkAuthFromDOM();
    
    timer(1000).pipe(takeUntil(this.destroy$)).subscribe(() => {
      this.authService.checkAuthStatus().subscribe();
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadUrlHistory(): void {
    this.isLoadingHistory = true;
    this.urlService.getUrlHistory().subscribe({
      next: (history) => {
        this.shortenedUrls = history.map(url => ({
          ...url,
          shortUrl: this.buildShortUrl(url.shortCode || url.id.toString()),
          createdAt: new Date(url.createdAt)
        }));
        this.isLoadingHistory = false;
      },
      error: (error) => {
        console.error('Error loading URL history:', error);
        this.shortenedUrls = [];
        this.isLoadingHistory = false;
      }
    });
  }

  canShortenUrl(): boolean {
    return !this.isLoading && 
           this.urlToShorten.trim().length > 0 && 
           this.currentUser.isAuthenticated === true;
  }

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

  shortenUrl(): void {
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
        this.clearMessages();
        this.showSuccess('Successfully logged out');
        this.shortenedUrls = [];
        
      },
      error: (error) => {
        console.error('Logout error:', error);
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