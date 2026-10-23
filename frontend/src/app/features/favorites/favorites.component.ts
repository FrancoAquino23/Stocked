import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';

import { RecipesService } from '../../core/services/recipes.service';
import { RecipeWithMatch } from '../../core/models/recipe.model';
import { AsyncViewState } from '../../shared/utils/async-view-state';
import { AsyncStateComponent } from '../../shared/components/async-state/async-state.component';
import { CUISINE_OPTIONS, DIET_OPTIONS, TYPE_OPTIONS } from '../../shared/utils/recipe-filter-options';

@Component({
  selector: 'app-favorites',
  standalone: true,
  imports: [RouterLink, FormsModule, AsyncStateComponent],
  templateUrl: './favorites.component.html',
  styleUrl: './favorites.component.scss',
})
export class FavoritesComponent {
  private readonly recipesService = inject(RecipesService);

  readonly cuisineOptions = CUISINE_OPTIONS;
  readonly dietOptions = DIET_OPTIONS;
  readonly typeOptions = TYPE_OPTIONS;

  recipes = signal<RecipeWithMatch[]>([]);
  state = signal<AsyncViewState>('loading');

  // Búsqueda y filtros: no requieren llamar al backend, ya se cachean en estas recetas
  query = signal('');
  cuisine = signal('');
  diet = signal('');
  type = signal('');

  // Filtrar las favoritas localmente por texto y por cocina/dieta/tipo
  filteredRecipes = computed(() => {
    const query = this.query().trim().toLowerCase();
    const cuisine = this.cuisine().toLowerCase();
    const diet = this.diet().toLowerCase();
    const type = this.type().toLowerCase();

    return this.recipes().filter((recipe) => {
      const matchesQuery = !query || recipe.name.toLowerCase().includes(query);
      const matchesCuisine = !cuisine || recipe.cuisines.some((value) => value.toLowerCase() === cuisine);
      const matchesDiet = !diet || recipe.diets.some((value) => value.toLowerCase() === diet);
      const matchesType = !type || recipe.dishTypes.some((value) => value.toLowerCase() === type);

      return matchesQuery && matchesCuisine && matchesDiet && matchesType;
    });
  });

  // Saber si hay al menos un filtro activo
  hasActiveFilters = computed(() => !!(this.query() || this.cuisine() || this.diet() || this.type()));

  viewMode = signal<'grid' | 'list'>('grid');

  constructor() {
    this.load();
  }

  // Cargar la lista desde el backend
  load(): void {
    this.state.set('loading');
    this.recipesService.getFavorites().subscribe({
      next: (recipes) => {
        this.recipes.set(recipes);
        this.state.set(recipes.length > 0 ? 'success' : 'empty');
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
  }
}
