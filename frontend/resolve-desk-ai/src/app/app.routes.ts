import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'dashboard'
  },
  {
    path: 'dashboard',
    title: 'Overview | ResolveDesk AI',
    loadComponent: () =>
      import('./features/dashboard/dashboard-page').then(
        module => module.DashboardPage
      )
  },
  {
    path: 'tickets',
    title: 'Tickets | ResolveDesk AI',
    loadComponent: () =>
      import('./features/tickets/tickets-page').then(
        module => module.TicketsPage
      )
  },
  {
    path: '**',
    redirectTo: 'dashboard'
  }
];
