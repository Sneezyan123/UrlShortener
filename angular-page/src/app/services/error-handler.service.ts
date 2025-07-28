import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ErrorHandlerService {
  handleError(error: any, context: string): void {
    console.error(`Error in ${context}:`, error);
  }

  getUserFriendlyErrorMessage(error: any): string {
    if (error.status === 0) {
      return 'Network error. Please check your connection.';
    }
    
    if (error.status >= 400 && error.status < 500) {
      return error.error?.message || 'Invalid request. Please try again.';
    }
    
    if (error.status >= 500) {
      return 'Server error. Please try again later.';
    }
    
    return 'An unexpected error occurred. Please try again.';
  }
}