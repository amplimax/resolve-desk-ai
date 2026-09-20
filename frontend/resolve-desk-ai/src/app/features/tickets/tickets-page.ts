import { DatePipe } from '@angular/common';
import { Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTableModule } from '@angular/material/table';
import { finalize } from 'rxjs';

import { SupportTicket } from '../../core/models/support-ticket';
import { SupportTicketApi } from '../../core/services/support-ticket-api';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-tickets-page',
  imports: [
    DatePipe,
    MatButtonModule,
    MatCardModule,
    MatProgressSpinnerModule,
    MatTableModule,
    RouterLink,
  ],
  templateUrl: './tickets-page.html',
  styleUrl: './tickets-page.scss',
})
export class TicketsPage {
  private readonly ticketApi = inject(SupportTicketApi);
  private readonly destroyRef = inject(DestroyRef);

  protected readonly tickets = signal<SupportTicket[]>([]);
  protected readonly loading = signal(false);
  protected readonly error = signal<string | null>(null);

  protected readonly displayedColumns: string[] = [
    'title',
    'requester',
    'status',
    'priority',
    'category',
    'createdAt',
  ];

  constructor() {
    this.loadTickets();
  }

  protected loadTickets(): void {
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
          this.error.set('Unable to load support tickets. Verify that the API is running.');
        },
      });
  }

  protected formatCategory(category: string): string {
    const labels: Record<string, string> = {
      AccountAccess: 'Account access',
      FeatureRequest: 'Feature request',
    };

    return labels[category] ?? category;
  }
}
