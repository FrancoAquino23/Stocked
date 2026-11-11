import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, catchError, throwError, timeout } from 'rxjs';

import { environment } from '../../../environments/environment';
import { ShoppingListItem } from '../models/shopping-list.model';

@Injectable({ providedIn: 'root' })
export class ShoppingListService {
  private readonly http = inject(HttpClient);

  // Calcular la lista de faltantes de la semana que arranca en weekStart (YYYY-MM-DD)
  getShoppingList(weekStart: string): Observable<ShoppingListItem[]> {
    return this.http.get<ShoppingListItem[]>(`${environment.apiUrl}/shopping-list`, { params: { weekStart } }).pipe(
      timeout(8000),
      catchError((error) => throwError(() => error)),
    );
  }
}
