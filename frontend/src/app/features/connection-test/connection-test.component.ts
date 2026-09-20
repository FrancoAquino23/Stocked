import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { catchError, of, timeout } from 'rxjs';
import { UsersService } from '../../core/services/users.service';
import { User } from '../../core/models/user.model';

// TEMP: Verificar la conectividad Postgres <-> API <-> Angular
type ConnectionTestState = 'loading' | 'error' | 'empty' | 'success';

@Component({
  selector: 'app-connection-test',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './connection-test.component.html',
  styleUrl: './connection-test.component.scss',
})
export class ConnectionTestComponent implements OnInit {
  private readonly usersService = inject(UsersService);

  state: ConnectionTestState = 'loading';
  users: User[] = [];

  // Cargar los usuarios al iniciar el componente
  ngOnInit(): void {
    this.loadUsers();
  }

  // Pedir la lista de usuarios a la API y actualizar el estado de la pantalla
  private loadUsers(): void {
    this.state = 'loading';

    this.usersService
      .getUsers()
      .pipe(
        timeout(8000),
        catchError(() => {
          this.state = 'error';
          return of<User[]>([]);
        })
      )
      .subscribe((users) => {
        if (this.state === 'error') {
          return;
        }

        this.users = users;
        this.state = users.length === 0 ? 'empty' : 'success';
      });
  }
}
