import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UrlShortenerComponent } from './components/url-shortener/url-shortener.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, UrlShortenerComponent],
  template: `
    <app-url-shortener></app-url-shortener>
  `
})
export class App {
  title = 'URL Shortener';
}