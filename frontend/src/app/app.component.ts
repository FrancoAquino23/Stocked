import { Component, HostListener, inject, signal } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

import { AuthService } from './core/services/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  title = 'Stocked';
  showLogoutConfirm = signal(false);

  // Pedir confirmación antes de cerrar sesión
  onLogoutClick(): void {
    this.showLogoutConfirm.set(true);
  }

  // Cancelar, se queda en la sesión actual
  cancelLogout(): void {
    this.showLogoutConfirm.set(false);
  }

  // Cerrar sesión y volver al login
  confirmLogout(): void {
    this.showLogoutConfirm.set(false);
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  // Cerrar el modal de confirmación con la tecla Escape
  @HostListener('document:keydown.escape')
  onEscape(): void {
    if (this.showLogoutConfirm()) {
      this.cancelLogout();
    }
  }
}
