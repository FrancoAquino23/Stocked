import { Routes } from '@angular/router';

import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () => import('./features/login/login.component').then((m) => m.LoginComponent),
  },
  {
    path: 'register',
    loadComponent: () => import('./features/register/register.component').then((m) => m.RegisterComponent),
  },
  {
    path: 'onboarding',
    canActivate: [authGuard],
    loadComponent: () => import('./features/onboarding/onboarding.component').then((m) => m.OnboardingComponent),
  },
  {
    path: 'recipes',
    canActivate: [authGuard],
    loadComponent: () => import('./features/recipe-search/recipe-search.component').then((m) => m.RecipeSearchComponent),
  },
  {
    path: 'recipes/favorites',
    canActivate: [authGuard],
    loadComponent: () => import('./features/favorites/favorites.component').then((m) => m.FavoritesComponent),
  },
  {
    path: 'recipes/recent',
    canActivate: [authGuard],
    loadComponent: () => import('./features/recent/recent.component').then((m) => m.RecentComponent),
  },
  {
    path: 'recipes/:externalId',
    canActivate: [authGuard],
    loadComponent: () => import('./features/recipe-detail/recipe-detail.component').then((m) => m.RecipeDetailComponent),
  },
  {
    path: 'pantry',
    canActivate: [authGuard],
    loadComponent: () => import('./features/pantry/pantry.component').then((m) => m.PantryComponent),
  },
  { path: '', pathMatch: 'full', redirectTo: 'recipes' },
  { path: '**', redirectTo: 'recipes' },
];
