import { DatePipe } from '@angular/common';
import { Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';
import { finalize } from 'rxjs';

import {
  SupportTicket,
  SupportTicketQuery,
  TicketCategory,
  TicketPriority,
  TicketStatus,
} from '../../core/models/support-ticket';
import { SupportTicketApi } from '../../core/services/support-ticket-api';

@Component({
  selector: 'app-tickets-page',
  imports: [
    DatePipe,
    MatButtonModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatProgressSpinnerModule,
    MatSelectModule,
    MatTableModule,
    ReactiveFormsModule,
    RouterLink,
  ],
  templateUrl: './tickets-page.html',
  styleUrl: './tickets-page.scss',
})
export class TicketsPage {
  private readonly formBuilder = inject(FormBuilder);
  private readonly ticketApi = inject(SupportTicketApi);
  private readonly destroyRef = inject(DestroyRef);

  protected readonly tickets = signal<SupportTicket[]>([]);
  protected readonly loading = signal(false);
  protected readonly error = signal<string | null>(null);

  protected readonly statuses: readonly TicketStatus[] = ['Open', 'InProgress', 'Resolved'];

  protected readonly priorities: readonly TicketPriority[] = ['Low', 'Medium', 'High', 'Critical'];

  protected readonly categories: readonly TicketCategory[] = [
    'Uncategorized',
    'Technical',
    'AccountAccess',
    'Billing',
    'FeatureRequest',
    'Other',
  ];

  protected readonly filters = this.formBuilder.nonNullable.group({
    search: '',
    status: '' as TicketStatus | '',
    priority: '' as TicketPriority | '',
    category: '' as TicketCategory | '',
  });

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

  protected applyFilters(): void {
    this.loadTickets();
  }

  protected resetFilters(): void {
    this.filters.reset({
      search: '',
      status: '',
      priority: '',
      category: '',
    });

    this.loadTickets();
  }

  protected loadTickets(): void {
    this.loading.set(true);
    this.error.set(null);

    this.ticketApi
      .getAll(this.buildQuery())
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

  protected formatStatus(status: TicketStatus): string {
    return status === 'InProgress' ? 'In progress' : status;
  }

  protected formatCategory(category: TicketCategory): string {
    const labels: Partial<Record<TicketCategory, string>> = {
      AccountAccess: 'Account access',
      FeatureRequest: 'Feature request',
    };

    return labels[category] ?? category;
  }

  private buildQuery(): SupportTicketQuery {
    const values = this.filters.getRawValue();
    const search = values.search.trim();

    return {
      search: search || undefined,
      status: values.status || undefined,
      priority: values.priority || undefined,
      category: values.category || undefined,
    };
  }
}
