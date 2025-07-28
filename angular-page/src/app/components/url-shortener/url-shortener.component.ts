import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subject, takeUntil } from 'rxjs';
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
  isLoading: boolean = false;
  errorMessage: string = '';
  successMessage: string = '';
  currentUser: UserInfo = { isAuthenticated: false };
  
  private destroy$ = new Subject<void>();

  constructor(
    private urlService: UrlService,
    private authService: AuthService,
    private navigationService: NavigationService,
    private errorHandler: ErrorHandlerService
  ) {}

  ngOnInit(): void {
    this.authService.user$
      .pipe(takeUntil(this.destroy$))
      .subscribe(user => {
        this.currentUser = user;
        this.loadUrlHistory();
      });
    
    this.authService.checkAuthFromDOM();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadUrlHistory(): void {
    this.isLoading = true;
    this.errorMessage = '';
    
    this.urlService.getUrlHistory().subscribe({
      next: (history) => {
        this.shortenedUrls = history.map(url => ({
          ...url,
          createdAt: new Date(url.createdAt)
        }));
        this.isLoading = false;
      },
      error: (error) => {
        this.errorHandler.handleError(error, 'loadUrlHistory');
        this.errorMessage = this.errorHandler.getUserFriendlyErrorMessage(error);
        this.isLoading = false;
      }
    });
  }

  shortenUrl(): void {
    if (!this.urlToShorten.trim()) {
      this.errorMessage = 'Please enter a valid URL';
      return;
    }

    if (!this.isValidUrl(this.urlToShorten)) {
      this.errorMessage = 'Please enter a valid URL format (e.g., https://example.com)';
      return;
    }

    if (!this.currentUser.isAuthenticated) {
      this.errorMessage = 'You must be signed in to shorten URLs';
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.urlService.shortenUrl({ url: this.urlToShorten }).subscribe({
      next: (response) => {
        this.shortenedUrls = [{
          ...response,
          createdAt: new Date(response.createdAt)
        }, ...this.shortenedUrls];
        
        this.urlToShorten = '';
        this.successMessage = 'URL shortened successfully!';
        this.isLoading = false;
        
        setTimeout(() => {
          this.successMessage = '';
        }, 3000);
      },
      error: (error) => {
        this.errorHandler.handleError(error, 'shortenUrl');
        
        if (error.status === 401) {
          this.errorMessage = 'You must be signed in to shorten URLs';
        } else {
          this.errorMessage = this.errorHandler.getUserFriendlyErrorMessage(error);
        }
        
        this.isLoading = false;
      }
    });
  }

  navigateToHome(): void {
    this.navigationService.navigateToHome();
  }

  navigateToAbout(): void {
    this.navigationService.navigateToAbout();
  }

  onSignIn(): void {
    this.navigationService.navigateToLogin();
  }

  onSignUp(): void {
    this.navigationService.navigateToRegister();
  }

  onSignOut(): void {
    window.location.href = '/user/logout';
  }

  clearError(): void {
    this.errorMessage = '';
  }

  clearSuccess(): void {
    this.successMessage = '';
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
      this.successMessage = 'URL copied to clipboard!';
      setTimeout(() => {
        this.successMessage = '';
      }, 2000);
    }).catch(() => {
      const textArea = document.createElement('textarea');
      textArea.value = url;
      document.body.appendChild(textArea);
      textArea.select();
      document.execCommand('copy');
      document.body.removeChild(textArea);
      
      this.successMessage = 'URL copied to clipboard!';
      setTimeout(() => {
        this.successMessage = '';
      }, 2000);
    });
  }
}