import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { Observable, catchError, tap, throwError, timeout } from 'rxjs';

import { environment } from '../../../environments/environment';
import { AuthResponse, LoginRequest, RegisterRequest, UpdatePantryScheduleRequest } from '../models/auth.model';

const STORAGE_KEY = 'stocked_auth';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly authState = signal<AuthResponse | null>(this.readFromStorage());

  readonly token = computed(() => this.authState()?.token ?? null);
  readonly username = computed(() => this.authState()?.username ?? null);
  readonly isAuthenticated = computed(() => !!this.authState()?.token);
  readonly hasCompletedOnboarding = computed(() => this.authState()?.pantryDayOfWeek != null);

  // Iniciar sesión
  login(request: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${environment.apiUrl}/auth/login`, request).pipe(
      timeout(8000),
      tap((response) => this.persist(response)),
      catchError((error) => throwError(() => error)),
    );
  }

  // Registrar una cuenta nueva
  register(request: RegisterRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${environment.apiUrl}/auth/register`, request).pipe(
      timeout(8000),
      tap((response) => this.persist(response)),
      catchError((error) => throwError(() => error)),
    );
  }

  // Guardar el día/hora de despensa
  completeOnboarding(request: UpdatePantryScheduleRequest): Observable<AuthResponse> {
    return this.http.put<AuthResponse>(`${environment.apiUrl}/auth/pantry-schedule`, request).pipe(
      timeout(8000),
      tap((response) => this.persist(response)),
      catchError((error) => throwError(() => error)),
    );
  }

  // Cerrar sesión
  logout(): void {
    this.authState.set(null);
    localStorage.removeItem(STORAGE_KEY);
  }

  // Guardar la sesión en memoria y en localStorage
  private persist(response: AuthResponse): void {
    this.authState.set(response);
    try {
      localStorage.setItem(STORAGE_KEY, JSON.stringify(response));
    } catch {
    }
  }

  // Leer la sesión guardada al iniciar la app
  private readFromStorage(): AuthResponse | null {
    try {
      const raw = localStorage.getItem(STORAGE_KEY);
      return raw ? (JSON.parse(raw) as AuthResponse) : null;
    } catch {
      return null;
    }
  }
}
