import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, catchError, throwError, timeout } from 'rxjs';

import { environment } from '../../../environments/environment';
import { getCurrentWeekStart } from '../../shared/utils/week';
import { RecipeAffordability, RecipePreparation } from '../models/recipe-preparation.model';

@Injectable({ providedIn: 'root' })
export class RecipePreparationsService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/recipe-preparations`;

  // Declarar que se va a preparar una receta X veces
  declare(externalId: number, multiplier: number): Observable<RecipePreparation> {
    return this.http.post<RecipePreparation>(this.baseUrl, { externalId, multiplier }).pipe(
      timeout(8000),
      catchError((error) => throwError(() => error)),
    );
  }

  // Consultar para cuántas de las veces solicitadas alcanza la despensa actual (nunca bloquea declarar)
  checkAffordability(externalId: number, multiplier: number): Observable<RecipeAffordability> {
    const weekStart = getCurrentWeekStart();
    return this.http
      .get<RecipeAffordability>(`${this.baseUrl}/affordability`, { params: { externalId, multiplier, weekStart } })
      .pipe(
        timeout(8000),
        catchError((error) => throwError(() => error)),
      );
  }
}
