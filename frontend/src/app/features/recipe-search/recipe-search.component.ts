import { Component, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { Observable, Subject, debounceTime, distinctUntilChanged, of, switchMap } from 'rxjs';

import { RecipesService } from '../../core/services/recipes.service';
import { RecipeSearchFilters, RecipeSearchResult } from '../../core/models/recipe.model';
import { AsyncViewState } from '../../shared/utils/async-view-state';
import { AsyncStateComponent } from '../../shared/components/async-state/async-state.component';
import { CUISINE_OPTIONS, DIET_OPTIONS, TYPE_OPTIONS } from '../../shared/utils/recipe-filter-options';

@Component({
  selector: 'app-recipe-search',
  standalone: true,
  imports: [RouterLink, FormsModule, AsyncStateComponent],
  templateUrl: './recipe-search.component.html',
  styleUrl: './recipe-search.component.scss',
})
export class RecipeSearchComponent {
  private readonly recipesService = inject(RecipesService);
  private readonly querySubject = new Subject<string>();

  // Recetas al azar cargadas una sola vez, para mostrarlas mientras no haya una búsqueda activa
  private defaultRecipes: RecipeSearchResult[] = [];

  readonly cuisineOptions = CUISINE_OPTIONS;
  readonly dietOptions = DIET_OPTIONS;
  readonly typeOptions = TYPE_OPTIONS;

  results = signal<RecipeSearchResult[]>([]);
  state = signal<AsyncViewState>('loading');
  hasSearched = signal(false);

  query = signal('');
  cuisine = signal('');
  diet = signal('');
  type = signal('');

  constructor() {
    this.loadDefaultRecipes();

    this.querySubject
      .pipe(
        debounceTime(300),
        distinctUntilChanged(),
        switchMap((query) => this.search$(query)),
        takeUntilDestroyed(),
      )
      .subscribe({
        next: (results) => {
          this.results.set(results);
          this.state.set(results.length > 0 ? 'success' : 'empty');
        },
        error: () => this.state.set('error'),
      });
  }

  // Cargar las recetas al azar que se muestran mientras no haya una búsqueda activa
  private loadDefaultRecipes(): void {
    this.recipesService.getRandom().subscribe({
      next: (recipes) => {
        this.defaultRecipes = recipes;
        this.results.set(recipes);
        this.state.set(recipes.length > 0 ? 'success' : 'empty');
      },
      error: () => this.state.set('error'),
    });
  }

  // Empaquetar los tres filtros activos en el formato que espera el servicio
  private currentFilters(): Partial<RecipeSearchFilters> {
    return { cuisine: this.cuisine(), diet: this.diet(), type: this.type() };
  }

  // Saber si hay al menos un filtro activo
  private hasActiveFilters(): boolean {
    return !!(this.cuisine() || this.diet() || this.type());
  }

  // Buscar de verdad solo si hay texto o algún filtro activo
  private search$(query: string): Observable<RecipeSearchResult[]> {
    const hasQuery = query.trim().length > 0;
    const hasFilters = this.hasActiveFilters();
    this.hasSearched.set(hasQuery || hasFilters);

    if (!hasQuery && !hasFilters) {
      return of(this.defaultRecipes);
    }

    this.state.set('loading');
    return this.recipesService.search(query.trim(), this.currentFilters());
  }

  // Reaccionar a cada cambio del campo de búsqueda
  onQueryChange(value: string): void {
    this.query.set(value);
    this.querySubject.next(value);
  }

  // Los filtros se aplican de inmediato, sin esperar a que cambie el texto de búsqueda
  onFilterChange(): void {
    this.search$(this.query()).subscribe({
      next: (results) => {
        this.results.set(results);
        this.state.set(results.length > 0 ? 'success' : 'empty');
      },
      error: () => this.state.set('error'),
    });
  }

  // Limpia el texto de búsqueda y los tres filtros a la vez
  clearFilters(): void {
    this.query.set('');
    this.cuisine.set('');
    this.diet.set('');
    this.type.set('');
    this.onFilterChange();
  }

  get emptyMessage(): string {
    return this.hasSearched() ? "We couldn't find any matching recipes." : 'Search for a recipe to get started.';
  }
}
