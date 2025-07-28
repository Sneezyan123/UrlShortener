import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

interface UrlHistory {
  id: number;
  originalUrl: string;
  shortUrl: string;
  createdAt: Date;
}

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  urlToShorten: string = '';
  shortenedUrls: UrlHistory[] = [];

  constructor(private http: HttpClient) {
    this.loadUrlHistory();
  }

  loadUrlHistory() {
    this.http.get<UrlHistory[]>('/api/urls/history')
      .subscribe({
        next: (history) => {
          this.shortenedUrls = history;
        },
        error: (error) => {
          console.error('Error loading URL history:', error);
        }
      });
  }

  shortenUrl() {
    if (!this.urlToShorten) return;

    this.http.post<UrlHistory>('/api/urls/shorten', { url: this.urlToShorten })
      .subscribe({
        next: (response) => {
          this.shortenedUrls = [response, ...this.shortenedUrls];
          this.urlToShorten = '';
        },
        error: (error) => {
          console.error('Error shortening URL:', error);
          // Here you could add error handling UI feedback
        }
      });
  }

  onSignIn() {
    // TODO: Implement sign in functionality
    console.log('Sign in clicked');
  }

  onSignUp() {
    // TODO: Implement sign up functionality
    console.log('Sign up clicked');
  }
}
