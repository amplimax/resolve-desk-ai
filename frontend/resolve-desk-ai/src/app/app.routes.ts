import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'dashboard',
  },
  {
    path: 'dashboard',
    title: 'Overview | ResolveDesk AI',
    loadComponent: () =>
      import('./features/dashboard/dashboard-page').then((module) => module.DashboardPage),
  },
  {
    path: 'tickets/new',
    loadComponent: () =>
      import('./features/tickets/ticket-create-page').then(
        (component) => component.TicketCreatePage,
      ),
  },
  {
    path: 'tickets/:id',
    loadComponent: () =>
      import('./features/tickets/ticket-detail-page').then(
        (component) => component.TicketDetailPage,
      ),
  },
  {
    path: 'tickets',
    title: 'Tickets | ResolveDesk AI',
    loadComponent: () =>
      import('./features/tickets/tickets-page').then((module) => module.TicketsPage),
  },
  {
    path: '**',
    redirectTo: 'dashboard',
  },
];
