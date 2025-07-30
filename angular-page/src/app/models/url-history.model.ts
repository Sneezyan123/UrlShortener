export interface UrlHistory {
  id: number | string;
  originalUrl: string;
  shortCode?: string;
  shortUrl: string;
  createdAt: Date;
  creatorEmail?: string;
}

export interface ShortenUrlRequest {
  url: string;
}