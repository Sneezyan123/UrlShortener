import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UrlService } from '../../services/url.service';
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
export class UrlShortenerComponent implements OnInit {
  urlToShorten: string = '';
  shortenedUrls: UrlHistory[] = [];
  isLoading: boolean = false;
  errorMessage: string = '';

  constructor(
    private urlService: UrlService,
    private navigationService: NavigationService,
    private errorHandler: ErrorHandlerService
  ) {}

  ngOnInit(): void {
    this.loadUrlHistory();
  }

  loadUrlHistory(): void {
    this.isLoading = true;
    this.errorMessage = '';
    
    this.urlService.getUrlHistory().subscribe({
      next: (history) => {
        this.shortenedUrls = history;
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

    this.isLoading = true;
    this.errorMessage = '';

    this.urlService.shortenUrl({ url: this.urlToShorten }).subscribe({
      next: (response) => {
        this.shortenedUrls = [response, ...this.shortenedUrls];
        this.urlToShorten = '';
        this.isLoading = false;
      },
      error: (error) => {
        this.errorHandler.handleError(error, 'shortenUrl');
        this.errorMessage = this.errorHandler.getUserFriendlyErrorMessage(error);
        this.isLoading = false;
      }
    });
  }

  // Navigation methods
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

  // Utility methods
  clearError(): void {
    this.errorMessage = '';
  }

  isValidUrl(url: string): boolean {
    try {
      new URL(url);
      return true;
    } catch {
      return false;
    }
  }
}