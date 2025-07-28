import { Routes } from '@angular/router';
import { LoginComponent } from './user/login.component';

export const routes: Routes = [
  {
    path: 'user/login',
    component: LoginComponent
  },
  {
    path: '',
    redirectTo: '/',
    pathMatch: 'full'
  }
];
