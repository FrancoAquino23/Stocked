import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

import { AuthService } from '../../core/services/auth.service';
import { PantryService } from '../../core/services/pantry.service';
import { IngredientSearchResult } from '../../core/models/ingredient.model';
import { IngredientPickerComponent } from '../../shared/components/ingredient-picker/ingredient-picker.component';
import { CapitalizePipe } from '../../shared/pipes/capitalize.pipe';

const DAYS_OF_WEEK = [
  { value: 'Monday', label: 'Monday' },
  { value: 'Tuesday', label: 'Tuesday' },
  { value: 'Wednesday', label: 'Wednesday' },
  { value: 'Thursday', label: 'Thursday' },
  { value: 'Friday', label: 'Friday' },
  { value: 'Saturday', label: 'Saturday' },
  { value: 'Sunday', label: 'Sunday' },
] as const;

@Component({
  selector: 'app-onboarding',
  standalone: true,
  imports: [FormsModule, IngredientPickerComponent, CapitalizePipe],
  templateUrl: './onboarding.component.html',
  styleUrl: './onboarding.component.scss',
})
export class OnboardingComponent {
  private readonly authService = inject(AuthService);
  private readonly pantryService = inject(PantryService);
  private readonly router = inject(Router);

  readonly daysOfWeek = DAYS_OF_WEEK;

  step = signal<1 | 2>(1);
  selectedIngredients = signal<IngredientSearchResult[]>([]);

  dayOfWeek = signal(DAYS_OF_WEEK[0].value as string);
  time = signal('18:00');
  submitting = signal(false);
  errorMessage = signal<string | null>(null);

  // Agregar un ingrediente elegido en el buscador, sin duplicarlo si ya está
  onIngredientSelected(ingredient: IngredientSearchResult): void {
    if (this.selectedIngredients().some((item) => item.externalId === ingredient.externalId)) {
      return;
    }

    this.pantryService.add(ingredient.externalId).subscribe({
      next: () => this.selectedIngredients.set([...this.selectedIngredients(), ingredient]),
    });
  }

  // Avanzar al segundo paso (día/hora de despensa)
  goToStep2(): void {
    this.step.set(2);
  }

  // Guardar el día/hora de despensa y terminar el onboarding
  finish(): void {
    this.submitting.set(true);
    this.errorMessage.set(null);

    this.authService
      .completeOnboarding({ dayOfWeek: this.dayOfWeek(), time: `${this.time()}:00` })
      .subscribe({
        next: () => {
          this.submitting.set(false);
          this.router.navigate(['/recipes']);
        },
        error: () => {
          this.submitting.set(false);
          this.errorMessage.set('Could not save your settings. Please try again.');
        },
      });
  }
}
