import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';

import { RecipesService } from '../../core/services/recipes.service';
import { RecipeWithMatch } from '../../core/models/recipe.model';
import { AsyncViewState } from '../../shared/utils/async-view-state';
import { AsyncStateComponent } from '../../shared/components/async-state/async-state.component';

@Component({
  selector: 'app-recent',
  standalone: true,
  imports: [RouterLink, AsyncStateComponent],
  templateUrl: './recent.component.html',
  styleUrl: './recent.component.scss',
})
export class RecentComponent {
  private readonly recipesService = inject(RecipesService);

  recipes = signal<RecipeWithMatch[]>([]);
  state = signal<AsyncViewState>('loading');

  constructor() {
    this.load();
  }

  // Cargar la lista desde el backend
  load(): void {
    this.state.set('loading');
    this.recipesService.getRecent().subscribe({
      next: (recipes) => {
        this.recipes.set(recipes);
        this.state.set(recipes.length > 0 ? 'success' : 'empty');
      },
      error: () => this.state.set('error'),
    });
  }
}
