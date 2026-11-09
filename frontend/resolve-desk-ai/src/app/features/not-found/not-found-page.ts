import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-not-found-page',
  imports: [RouterLink, MatButtonModule],
  template: `
    <p>404</p>
    <h1>Page not found</h1>
    <p>The address may be incorrect, or this page or ticket is no longer available.</p>
    <a mat-flat-button routerLink="/">Back to home</a>
  `,
})
export class NotFoundPage {}
