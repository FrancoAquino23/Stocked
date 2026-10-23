import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, catchError, throwError, timeout } from 'rxjs';

import { environment } from '../../../environments/environment';
import { PantryItem } from '../models/pantry.model';

@Injectable({ providedIn: 'root' })
export class PantryService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/pantry-items`;

  // Listar la despensa completa
  getAll(): Observable<PantryItem[]> {
    return this.http.get<PantryItem[]>(this.baseUrl).pipe(
      timeout(8000),
      catchError((error) => throwError(() => error)),
    );
  }

  // Agregar un ingrediente a la despensa
  add(externalId: number): Observable<PantryItem> {
    return this.http.post<PantryItem>(this.baseUrl, { externalId }).pipe(
      timeout(8000),
      catchError((error) => throwError(() => error)),
    );
  }

  // Capturar/actualizar la cantidad habitual de compra
  update(externalId: number, usualQuantity: number | null, usualUnit: string | null): Observable<PantryItem> {
    return this.http.put<PantryItem>(`${this.baseUrl}/${externalId}`, { usualQuantity, usualUnit }).pipe(
      timeout(8000),
      catchError((error) => throwError(() => error)),
    );
  }

  // Quitar un ingrediente de la despensa
  remove(externalId: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${externalId}`).pipe(
      timeout(8000),
      catchError((error) => throwError(() => error)),
    );
  }
}
