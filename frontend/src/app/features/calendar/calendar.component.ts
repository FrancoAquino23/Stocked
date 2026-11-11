import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';

import { AuthService } from '../../core/services/auth.service';
import { ShoppingListService } from '../../core/services/shopping-list.service';
import { ShoppingListItem } from '../../core/models/shopping-list.model';
import { AsyncViewState } from '../../shared/utils/async-view-state';
import { AsyncStateComponent } from '../../shared/components/async-state/async-state.component';
import { CapitalizePipe } from '../../shared/pipes/capitalize.pipe';
import { DAYS_OF_WEEK } from '../../shared/utils/days-of-week';
import { getCurrentWeekStart } from '../../shared/utils/week';

const MONTH_NAMES = [
  'January', 'February', 'March', 'April', 'May', 'June',
  'July', 'August', 'September', 'October', 'November', 'December',
];

interface CalendarCell {
  day: number;
  isPantryDay: boolean;
  isToday: boolean;
}

@Component({
  selector: 'app-calendar',
  standalone: true,
  imports: [FormsModule, RouterLink, AsyncStateComponent, CapitalizePipe],
  templateUrl: './calendar.component.html',
  styleUrl: './calendar.component.scss',
})
export class CalendarComponent {
  private readonly authService = inject(AuthService);
  private readonly shoppingListService = inject(ShoppingListService);

  readonly daysOfWeek = DAYS_OF_WEEK;
  readonly weekdayLabels = DAYS_OF_WEEK.map((day) => day.label.slice(0, 3));

  private readonly today = new Date();
  viewYear = signal(this.today.getFullYear());
  viewMonth = signal(this.today.getMonth());

  dayOfWeek = signal(this.authService.pantryDayOfWeek() ?? DAYS_OF_WEEK[0].value);
  time = signal((this.authService.pantryTime() ?? '18:00:00').slice(0, 5));
  savingSchedule = signal(false);
  scheduleSavedMessage = signal<string | null>(null);

  items = signal<ShoppingListItem[]>([]);
  loadState = signal<AsyncViewState>('loading');
  dismissedNames = signal<Set<string>>(new Set());

  // Ingredientes que calculó el backend, sin los que el usuario descartó localmente
  visibleItems = computed(() => this.items().filter((item) => !this.dismissedNames().has(item.ingredientName)));

  // Una vez cargada la lista, "empty" si ya no queda nada visible (incluye lo descartado a mano)
  displayState = computed<AsyncViewState>(() => {
    if (this.loadState() !== 'success') {
      return this.loadState();
    }
    return this.visibleItems().length > 0 ? 'success' : 'empty';
  });

  // Texto del encabezado, ej. "November 2026"
  monthLabel = computed(() => `${MONTH_NAMES[this.viewMonth()]} ${this.viewYear()}`);

  // Semanas del mes visible (lunes a domingo), con huecos vacíos antes/después para alinear la grid
  weeks = computed<(CalendarCell | null)[][]>(() => {
    const year = this.viewYear();
    const month = this.viewMonth();
    const daysInMonth = new Date(year, month + 1, 0).getDate();
    const firstWeekdayJs = new Date(year, month, 1).getDay();
    const leadingBlanks = firstWeekdayJs === 0 ? 6 : firstWeekdayJs - 1;

    const cells: (CalendarCell | null)[] = Array.from({ length: leadingBlanks }, () => null);
    for (let day = 1; day <= daysInMonth; day++) {
      const date = new Date(year, month, day);
      const jsDayIndex = date.getDay();
      const dayName = DAYS_OF_WEEK[jsDayIndex === 0 ? 6 : jsDayIndex - 1].value;

      cells.push({
        day,
        isPantryDay: dayName === this.dayOfWeek(),
        isToday:
          day === this.today.getDate() && month === this.today.getMonth() && year === this.today.getFullYear(),
      });
    }
    while (cells.length % 7 !== 0) {
      cells.push(null);
    }

    const weeks: (CalendarCell | null)[][] = [];
    for (let i = 0; i < cells.length; i += 7) {
      weeks.push(cells.slice(i, i + 7));
    }
    return weeks;
  });

  // Ir al mes anterior
  prevMonth(): void {
    this.shiftMonth(-1);
  }

  // Ir al mes siguiente
  nextMonth(): void {
    this.shiftMonth(1);
  }

  // Mover el mes visible hacia adelante o hacia atrás
  private shiftMonth(delta: number): void {
    const date = new Date(this.viewYear(), this.viewMonth() + delta, 1);
    this.viewYear.set(date.getFullYear());
    this.viewMonth.set(date.getMonth());
  }

  constructor() {
    this.loadShoppingList();
  }

  // Guardar el día/hora de despensa (recurrente) — mismo endpoint que el onboarding
  saveSchedule(): void {
    this.savingSchedule.set(true);
    this.scheduleSavedMessage.set(null);

    this.authService.updatePantrySchedule({ dayOfWeek: this.dayOfWeek(), time: `${this.time()}:00` }).subscribe({
      next: () => {
        this.savingSchedule.set(false);
        this.scheduleSavedMessage.set('Pantry day saved.');
      },
      error: () => {
        this.savingSchedule.set(false);
        this.scheduleSavedMessage.set('Could not save your pantry day. Please try again.');
      },
    });
  }

  // Calcular la lista de faltantes de la semana que arranca hoy
  loadShoppingList(): void {
    this.loadState.set('loading');
    this.shoppingListService.getShoppingList(getCurrentWeekStart()).subscribe({
      next: (items) => {
        this.items.set(items);
        this.dismissedNames.set(new Set());
        this.loadState.set('success');
      },
      error: () => this.loadState.set('error'),
    });
  }

  // Quitar un ingrediente calculado de la vista — no se guarda, es solo para esta sesión
  dismiss(ingredientName: string): void {
    this.dismissedNames.set(new Set([...this.dismissedNames(), ingredientName]));
  }
}
