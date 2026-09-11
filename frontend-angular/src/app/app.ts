import { Component, inject, signal } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

import { AuthSessionService } from './core/services/auth-session.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected readonly session = inject(AuthSessionService);
  private readonly router = inject(Router);

  protected readonly title = signal('frontend-angular');

  protected cerrarSesion(): void {
    this.session.cerrarSesion();
    this.router.navigateByUrl('/login');
  }
}
