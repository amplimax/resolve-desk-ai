import { Routes, UrlSegment } from '@angular/router';

const hasTicketId = (_route: unknown, segments: UrlSegment[]) =>
  /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i.test(segments[1]?.path ?? '');

export const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    title: 'ResolveDesk AI | AI-assisted support ticket management',
    data: { indexable: true },
    loadComponent: () =>
      import('./features/landing/landing-page').then((module) => module.LandingPage),
  },
  {
    path: 'dashboard',
    title: 'Overview | ResolveDesk AI',
    loadComponent: () =>
      import('./features/dashboard/dashboard-page').then((module) => module.DashboardPage),
  },
  {
    path: 'tickets/new',
    title: 'Create ticket | ResolveDesk AI',
    loadComponent: () =>
      import('./features/tickets/ticket-create-page').then(
        (component) => component.TicketCreatePage,
      ),
  },
  {
    path: 'tickets/:id/edit',
    title: 'Edit ticket | ResolveDesk AI',
    canMatch: [hasTicketId],
    loadComponent: () =>
      import('./features/tickets/ticket-edit-page').then((component) => component.TicketEditPage),
  },
  {
    path: 'tickets/:id',
    title: 'Ticket details | ResolveDesk AI',
    canMatch: [hasTicketId],
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
    path: 'not-found',
    title: 'Page not found | ResolveDesk AI',
    loadComponent: () =>
      import('./features/not-found/not-found-page').then((module) => module.NotFoundPage),
  },
  {
    path: '**',
    title: 'Page not found | ResolveDesk AI',
    loadComponent: () =>
      import('./features/not-found/not-found-page').then((module) => module.NotFoundPage),
  },
];
