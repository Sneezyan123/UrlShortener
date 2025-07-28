export interface UrlHistory {
  id: number;
  originalUrl: string;
  shortUrl: string;
  createdAt: Date;
}

export interface ShortenUrlRequest {
  url: string;
}