import { Injectable } from '@angular/core';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class NavigationService {
  constructor(private router: Router) {}

  navigateToHome(): void {
    window.location.href = '/home/index';
  }

  navigateToAbout(): void {
    window.location.href = '/home/about';
  }

  navigateToLogin(): void {
    window.location.href = '/user/login';
  }

  navigateToRegister(): void {
    window.location.href = '/user/register';
  }
}