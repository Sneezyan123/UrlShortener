import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { UrlShortenerComponent } from './components/url-shortener/url-shortener.component';

const routes: Routes = [
  { 
    path: 'shorten', 
    component: UrlShortenerComponent
  },
  { 
    path: '', 
    redirectTo: '/shorten', 
    pathMatch: 'full' 
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
