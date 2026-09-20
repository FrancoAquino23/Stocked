import { Component } from '@angular/core';
// TEMP: Verificar la conectividad Postgres <-> API <-> Angular
import { ConnectionTestComponent } from './features/connection-test/connection-test.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [ConnectionTestComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'Stocked';
}
