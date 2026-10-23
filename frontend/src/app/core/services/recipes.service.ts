import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, catchError, throwError, timeout } from 'rxjs';

import { environment } from '../../../environments/environment';
import { RecipeDetail, RecipeSearchFilters, RecipeSearchResult, RecipeWithMatch } from '../models/recipe.model';

@Injectable({ providedIn: 'root' })
export class RecipesService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/recipes`;

  // Buscar recetas por texto libre, con filtros opcionales de cocina/dieta/tipo de comida
  search(query: string, filters?: Partial<RecipeSearchFilters>): Observable<RecipeSearchResult[]> {
    const params: Record<string, string> = { query };
    if (filters?.cuisine) {
      params['cuisine'] = filters.cuisine;
    }
    if (filters?.diet) {
      params['diet'] = filters.diet;
    }
    if (filters?.type) {
      params['type'] = filters.type;
    }

    return this.http.get<RecipeSearchResult[]>(`${this.baseUrl}/search`, { params }).pipe(
      timeout(8000),
      catchError((error) => throwError(() => error)),
    );
  }

  // Recetas al azar, para que la pantalla de búsqueda no arranque vacía
  getRandom(): Observable<RecipeSearchResult[]> {
    return this.http.get<RecipeSearchResult[]>(`${this.baseUrl}/random`).pipe(
      timeout(8000),
      catchError((error) => throwError(() => error)),
    );
  }

  // Ver el detalle completo de una receta
  getDetail(externalId: number): Observable<RecipeDetail> {
    return this.http.get<RecipeDetail>(`${this.baseUrl}/${externalId}`).pipe(
      timeout(8000),
      catchError((error) => throwError(() => error)),
    );
  }

  // Marcar una receta como favorita
  addFavorite(externalId: number): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/${externalId}/favorite`, {}).pipe(
      timeout(8000),
      catchError((error) => throwError(() => error)),
    );
  }

  // Quitar una receta de favoritas
  removeFavorite(externalId: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${externalId}/favorite`).pipe(
      timeout(8000),
      catchError((error) => throwError(() => error)),
    );
  }

  // Listar las recetas favoritas
  getFavorites(): Observable<RecipeWithMatch[]> {
    return this.http.get<RecipeWithMatch[]>(`${this.baseUrl}/favorites`).pipe(
      timeout(8000),
      catchError((error) => throwError(() => error)),
    );
  }

  // Listar las recetas vistas recientemente
  getRecent(): Observable<RecipeWithMatch[]> {
    return this.http.get<RecipeWithMatch[]>(`${this.baseUrl}/recent`).pipe(
      timeout(8000),
      catchError((error) => throwError(() => error)),
    );
  }
}
