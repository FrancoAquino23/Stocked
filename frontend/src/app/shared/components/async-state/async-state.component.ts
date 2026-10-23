import { Component, input, output } from '@angular/core';

import { AsyncViewState } from '../../utils/async-view-state';

// Envuelve el contenido de una pantalla con sus 4 estados estándar (loading/error/empty/success)
@Component({
  selector: 'app-async-state',
  standalone: true,
  templateUrl: './async-state.component.html',
  styleUrl: './async-state.component.scss',
})
export class AsyncStateComponent {
  state = input.required<AsyncViewState>();
  emptyMessage = input('Nothing to show yet.');
  errorMessage = input('Something went wrong. Please try again.');
  retry = output<void>();
}
