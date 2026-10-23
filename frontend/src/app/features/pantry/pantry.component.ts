import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { PantryService } from '../../core/services/pantry.service';
import { IngredientSearchResult } from '../../core/models/ingredient.model';
import { PantryItem } from '../../core/models/pantry.model';
import { AsyncViewState } from '../../shared/utils/async-view-state';
import { AsyncStateComponent } from '../../shared/components/async-state/async-state.component';
import { IngredientPickerComponent } from '../../shared/components/ingredient-picker/ingredient-picker.component';
import { CapitalizePipe } from '../../shared/pipes/capitalize.pipe';

interface UndoableRemoval {
  item: PantryItem;
  timeoutId: ReturnType<typeof setTimeout>;
}

@Component({
  selector: 'app-pantry',
  standalone: true,
  imports: [FormsModule, AsyncStateComponent, IngredientPickerComponent, CapitalizePipe],
  templateUrl: './pantry.component.html',
  styleUrl: './pantry.component.scss',
})
export class PantryComponent {
  private readonly pantryService = inject(PantryService);
  private pendingRemoval: UndoableRemoval | null = null;

  items = signal<PantryItem[]>([]);
  state = signal<AsyncViewState>('loading');
  pendingRemovalId = signal<number | null>(null);

  constructor() {
    this.load();
  }

  // Cargar la lista desde el backend
  load(): void {
    this.state.set('loading');
    this.pantryService.getAll().subscribe({
      next: (items) => {
        this.items.set(items);
        this.state.set(items.length > 0 ? 'success' : 'empty');
      },
      error: () => this.state.set('error'),
    });
  }

  // Agregar un ingrediente elegido en el buscador, sin duplicarlo si ya está
  onIngredientSelected(ingredient: IngredientSearchResult): void {
    if (this.items().some((item) => item.externalId === ingredient.externalId)) {
      return;
    }

    this.pantryService.add(ingredient.externalId).subscribe({
      next: (item) => {
        this.items.set([...this.items(), item]);
        this.state.set('success');
      },
    });
  }

  // Actualizar la cantidad/unidad habitual de un ingrediente
  updateQuantity(item: PantryItem, usualQuantity: string, usualUnit: string): void {
    const quantity = usualQuantity.trim() ? Number(usualQuantity) : null;
    const unit = usualUnit.trim() || null;

    this.pantryService.update(item.externalId, quantity, unit).subscribe({
      next: (updated) => {
        this.items.set(this.items().map((current) => (current.externalId === updated.externalId ? updated : current)));
      },
    });
  }

  // Eliminar un ingrediente
  remove(item: PantryItem): void {
    this.finalizePendingRemoval();

    this.pantryService.remove(item.externalId).subscribe();

    const timeoutId = setTimeout(() => this.finalizePendingRemoval(), 5000);
    this.pendingRemoval = { item, timeoutId };
    this.pendingRemovalId.set(item.externalId);
  }

  // Deshacer la eliminación pendiente y restaurar el ingrediente
  undoRemoval(): void {
    if (!this.pendingRemoval) {
      return;
    }

    clearTimeout(this.pendingRemoval.timeoutId);
    const { item } = this.pendingRemoval;
    this.pendingRemoval = null;
    this.pendingRemovalId.set(null);

    this.pantryService.add(item.externalId).subscribe();
  }

  // Confirmar la eliminación pendiente
  private finalizePendingRemoval(): void {
    if (!this.pendingRemoval) {
      return;
    }

    const { item } = this.pendingRemoval;
    this.pendingRemoval = null;
    this.pendingRemovalId.set(null);
    this.items.set(this.items().filter((current) => current.externalId !== item.externalId));
    if (this.items().length === 0) {
      this.state.set('empty');
    }
  }
}
