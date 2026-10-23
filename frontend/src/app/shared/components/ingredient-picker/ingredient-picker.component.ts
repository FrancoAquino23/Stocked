import { Component, inject, output, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { EMPTY, Subject, catchError, debounceTime, distinctUntilChanged, switchMap } from 'rxjs';

import { IngredientsService } from '../../../core/services/ingredients.service';
import { IngredientSearchResult } from '../../../core/models/ingredient.model';
import { AsyncViewState } from '../../utils/async-view-state';
import { AsyncStateComponent } from '../async-state/async-state.component';
import { CapitalizePipe } from '../../pipes/capitalize.pipe';

// Buscador de ingredientes
@Component({
  selector: 'app-ingredient-picker',
  standalone: true,
  imports: [AsyncStateComponent, CapitalizePipe],
  templateUrl: './ingredient-picker.component.html',
  styleUrl: './ingredient-picker.component.scss',
})
export class IngredientPickerComponent {
  private readonly ingredientsService = inject(IngredientsService);
  private readonly querySubject = new Subject<string>();

  results = signal<IngredientSearchResult[]>([]);
  state = signal<AsyncViewState>('empty');
  hasSearched = signal(false);
  ingredientSelected = output<IngredientSearchResult>();
  private lastQuery = '';

  constructor() {
    this.querySubject
      .pipe(
        debounceTime(300),
        distinctUntilChanged(),
        switchMap((query) => {
          if (!query.trim()) {
            return [];
          }
          this.lastQuery = query.trim();
          return this.ingredientsService.search(this.lastQuery).pipe(
            catchError(() => {
              this.state.set('error');
              return EMPTY;
            }),
          );
        }),
        takeUntilDestroyed(),
      )
      .subscribe((results) => {
        this.hasSearched.set(true);
        this.results.set(results);
        this.state.set(results.length > 0 ? 'success' : 'empty');
      });
  }

  // Reaccionar a cada cambio del campo de búsqueda
  onQueryChange(value: string): void {
    if (!value.trim()) {
      this.hasSearched.set(false);
      this.results.set([]);
      this.state.set('empty');
    }
    this.querySubject.next(value);
  }

  // Reintentar la última búsqueda tras un error
  retry(): void {
    if (this.lastQuery) {
      this.querySubject.next(this.lastQuery);
    }
  }

  // Seleccionar un ingrediente y limpiar la búsqueda
  onSelect(result: IngredientSearchResult): void {
    this.ingredientSelected.emit(result);
    this.hasSearched.set(false);
    this.results.set([]);
    this.state.set('empty');
  }

  get emptyMessage(): string {
    return "We couldn't find ingredients with that name.";
  }

  get errorMessage(): string {
    return "We couldn't load ingredients. Please try again.";
  }
}
