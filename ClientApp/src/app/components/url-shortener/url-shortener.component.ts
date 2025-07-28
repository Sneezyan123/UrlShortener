import { Component } from '@angular/core';
import { UriService } from '../../services/uri.service';
import { UrlResponse } from '../../models/url.model';

@Component({
  selector: 'app-url-shortener',
  templateUrl: './url-shortener.component.html'
})
export class UrlShortenerComponent {
  longUrl: string = '';
  shortUrl: string = '';
  error: string = '';

  constructor(private uriService: UriService) {}

  shortenUrl() {
    if (!this.uriService.isAuthenticated()) {
      this.error = 'Please login first';
      return;
    }

    this.uriService.shortenUrl(this.longUrl).subscribe(
      (response: UrlResponse) => {
        this.shortUrl = response.shortUrl;
        this.error = '';
      },
      error => {
        this.error = error.message;
      }
    );
  }
}
