import { Component, HostListener, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Router } from '@angular/router';
import { Location } from '@angular/common';
import { Subject, debounceTime, distinctUntilChanged, switchMap } from 'rxjs';

import { RecipesService } from '../../core/services/recipes.service';
import { RecipePreparationsService } from '../../core/services/recipe-preparations.service';
import { RecipeDetail } from '../../core/models/recipe.model';
import { RecipeAffordability } from '../../core/models/recipe-preparation.model';
import { CapitalizePipe } from '../../shared/pipes/capitalize.pipe';

@Component({
  selector: 'app-recipe-detail',
  standalone: true,
  imports: [CapitalizePipe],
  templateUrl: './recipe-detail.component.html',
  styleUrl: './recipe-detail.component.scss',
})
export class RecipeDetailComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly location = inject(Location);
  private readonly recipesService = inject(RecipesService);
  private readonly recipePreparationsService = inject(RecipePreparationsService);
  private readonly multiplierSubject = new Subject<number>();

  private readonly externalId = Number(this.route.snapshot.paramMap.get('externalId'));

  recipe = signal<RecipeDetail | null>(null);
  loading = signal(true);
  loadError = signal(false);
  togglingFavorite = signal(false);

  multiplier = signal(1);
  affordability = signal<RecipeAffordability | null>(null);

  declaring = signal(false);
  declaredMessage = signal<string | null>(null);
  declareFailed = signal(false);
  showAffordabilityWarning = signal(false);

  constructor() {
    this.loadRecipe();

    this.multiplierSubject
      .pipe(
        debounceTime(300),
        distinctUntilChanged(),
        switchMap((multiplier) => this.recipePreparationsService.checkAffordability(this.externalId, multiplier)),
        takeUntilDestroyed(),
      )
      .subscribe({
        next: (affordability) => this.affordability.set(affordability),
      });
  }

  // Cargar la receta y preparar su resumen/pasos para mostrarse
  loadRecipe(): void {
    this.loading.set(true);
    this.loadError.set(false);
    this.recipesService.getDetail(this.externalId).subscribe({
      next: (recipe) => {
        const ingredientNames = recipe.ingredients.map((ingredient) => ingredient.name);
        this.recipe.set({
          ...recipe,
          summary: this.stripLinks(recipe.summary),
          steps: recipe.steps.map((step) => ({
            ...step,
            description: this.highlightIngredients(step.description, ingredientNames),
          })),
        });
        this.loading.set(false);
        this.multiplierSubject.next(this.multiplier());
      },
      error: () => {
        this.loading.set(false);
        this.loadError.set(true);
      },
    });
  }

  // Quita los <a> del resumen de Spoonacular, conservando el resto del formato
  private stripLinks(html: string): string {
    const container = document.createElement('div');
    container.innerHTML = html;
    container.querySelectorAll('a').forEach((anchor) => {
      anchor.replaceWith(...Array.from(anchor.childNodes));
    });
    return container.innerHTML;
  }

  // Resaltar en negritas los nombres de ingrediente que aparecen en el texto de un paso
  private highlightIngredients(text: string, ingredientNames: string[]): string {
    const escaped = ingredientNames
      .filter((name) => name.trim().length > 0)
      .sort((a, b) => b.length - a.length)
      .map((name) => name.replace(/[.*+?^${}()|[\]\\]/g, '\\$&'));

    if (escaped.length === 0) {
      return text;
    }

    const pattern = new RegExp(`\\b(${escaped.join('|')})\\b`, 'gi');
    return text.replace(pattern, '<strong>$1</strong>');
  }

  // Si se navegó desde dentro de la app regresa a esa pantalla
  goBack(): void {
    if (window.history.length > 1) {
      this.location.back();
    } else {
      this.router.navigateByUrl('/recipes');
    }
  }

  // Marcar o quitar la receta de favoritos
  toggleFavorite(): void {
    const recipe = this.recipe();
    if (!recipe || this.togglingFavorite()) {
      return;
    }

    this.togglingFavorite.set(true);
    const action = recipe.isFavorite
      ? this.recipesService.removeFavorite(recipe.externalId)
      : this.recipesService.addFavorite(recipe.externalId);

    action.subscribe({
      next: () => {
        this.recipe.set({ ...recipe, isFavorite: !recipe.isFavorite });
        this.togglingFavorite.set(false);
      },
      error: () => this.togglingFavorite.set(false),
    });
  }

  // Reaccionar a cada cambio del multiplicador
  onMultiplierChange(value: string): void {
    const parsed = Math.max(1, Number(value) || 1);
    this.multiplier.set(parsed);
    this.multiplierSubject.next(parsed);
  }

  // Avisar antes de declarar solo si la despensa no cubre todo
  onDeclareClick(): void {
    const current = this.affordability();
    if (current && !current.fullyCovered) {
      this.showAffordabilityWarning.set(true);
      return;
    }

    this.declare();
  }

  // Cerrar el modal de advertencia sin declarar
  closeAffordabilityWarning(): void {
    this.showAffordabilityWarning.set(false);
  }

  // Declarar la preparación aunque la despensa no alcance
  confirmDeclareAnyway(): void {
    this.showAffordabilityWarning.set(false);
    this.declare();
  }

  // Cerrar el modal de advertencia con la tecla Escape
  @HostListener('document:keydown.escape')
  onEscape(): void {
    if (this.showAffordabilityWarning()) {
      this.closeAffordabilityWarning();
    }
  }

  // Declarar la preparación de esta receta
  declare(): void {
    this.declaring.set(true);
    this.declaredMessage.set(null);
    this.declareFailed.set(false);

    this.recipePreparationsService.declare(this.externalId, this.multiplier()).subscribe({
      next: (preparation) => {
        this.declaring.set(false);
        this.declaredMessage.set(`Preparation declared: ${preparation.status}.`);
      },
      error: () => {
        this.declaring.set(false);
        this.declareFailed.set(true);
        this.declaredMessage.set('Could not declare the preparation. Please try again.');
      },
    });
  }
}
