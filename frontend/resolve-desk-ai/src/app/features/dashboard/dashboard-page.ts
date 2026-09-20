import { DatePipe } from '@angular/common';
import { Component, computed, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { finalize } from 'rxjs';

import { SupportTicket } from '../../core/models/support-ticket';
import { SupportTicketApi } from '../../core/services/support-ticket-api';

@Component({
  selector: 'app-dashboard-page',
  imports: [DatePipe, MatButtonModule, MatCardModule, MatProgressSpinnerModule, RouterLink],
  templateUrl: './dashboard-page.html',
  styleUrl: './dashboard-page.scss',
})
export class DashboardPage {
  private readonly ticketApi = inject(SupportTicketApi);
  private readonly destroyRef = inject(DestroyRef);

  protected readonly tickets = signal<SupportTicket[]>([]);
  protected readonly loading = signal(false);
  protected readonly error = signal<string | null>(null);

  protected readonly openCount = computed(
    () => this.tickets().filter((ticket) => ticket.status === 'Open').length,
  );

  protected readonly inProgressCount = computed(
    () => this.tickets().filter((ticket) => ticket.status === 'InProgress').length,
  );

  protected readonly resolvedCount = computed(
    () => this.tickets().filter((ticket) => ticket.status === 'Resolved').length,
  );

  protected readonly urgentCount = computed(
    () =>
      this.tickets().filter(
        (ticket) => ticket.priority === 'High' || ticket.priority === 'Critical',
      ).length,
  );

  protected readonly recentTickets = computed(() => this.tickets().slice(0, 5));

  constructor() {
    this.load();
  }

  protected load(): void {
    this.loading.set(true);
    this.error.set(null);

    this.ticketApi
      .getAll()
      .pipe(
        takeUntilDestroyed(this.destroyRef),
        finalize(() => this.loading.set(false)),
      )
      .subscribe({
        next: (tickets) => this.tickets.set(tickets),
        error: () => {
          this.error.set('Unable to load dashboard data. Verify that the API is running.');
        },
      });
  }

  protected formatStatus(status: string): string {
    return status === 'InProgress' ? 'In progress' : status;
  }
}
