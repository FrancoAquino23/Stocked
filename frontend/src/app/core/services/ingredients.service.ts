import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, catchError, of, timeout } from 'rxjs';

import { environment } from '../../../environments/environment';
import { IngredientSearchResult } from '../models/ingredient.model';

@Injectable({ providedIn: 'root' })
export class IngredientsService {
  private readonly http = inject(HttpClient);

  // Autocompletar ingredientes por texto libre
  search(query: string): Observable<IngredientSearchResult[]> {
    return this.http
      .get<IngredientSearchResult[]>(`${environment.apiUrl}/ingredients/search`, { params: { query } })
      .pipe(
        timeout(5000),
        catchError(() => of<IngredientSearchResult[]>([])),
      );
  }
}
