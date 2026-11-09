import { Component, DestroyRef, inject, signal } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { finalize } from 'rxjs';

import {
  TicketCategory,
  TicketPriority,
  TicketStatus,
  UpdateSupportTicketRequest,
} from '../../core/models/support-ticket';
import { SupportTicketApi } from '../../core/services/support-ticket-api';

@Component({
  selector: 'app-ticket-edit-page',
  imports: [
    MatButtonModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatProgressSpinnerModule,
    MatSelectModule,
    ReactiveFormsModule,
    RouterLink,
  ],
  templateUrl: './ticket-edit-page.html',
  styleUrl: './ticket-edit-page.scss',
})
export class TicketEditPage {
  private readonly formBuilder = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly ticketApi = inject(SupportTicketApi);
  private readonly destroyRef = inject(DestroyRef);

  protected readonly ticketId = this.route.snapshot.paramMap.get('id') ?? '';

  protected readonly loading = signal(false);
  protected readonly submitting = signal(false);
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

  protected readonly form = this.formBuilder.nonNullable.group({
    title: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(200)]],
    description: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(4000)]],
    requesterName: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(120)]],
    requesterEmail: ['', [Validators.required, Validators.email, Validators.maxLength(254)]],
    status: 'Open' as TicketStatus,
    priority: 'Medium' as TicketPriority,
    category: 'Uncategorized' as TicketCategory,
  });

  constructor() {
    this.load();
  }

  protected submit(): void {
    if (this.form.invalid || this.submitting()) {
      this.form.markAllAsTouched();
      return;
    }

    this.submitting.set(true);
    this.error.set(null);

    const request: UpdateSupportTicketRequest = this.form.getRawValue();

    this.ticketApi
      .update(this.ticketId, request)
      .pipe(
        takeUntilDestroyed(this.destroyRef),
        finalize(() => this.submitting.set(false)),
      )
      .subscribe({
        next: (ticket) => void this.router.navigate(['/tickets', ticket.id]),
        error: () => {
          this.error.set('Unable to update the ticket. Verify the entered data and try again.');
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

  private load(): void {
    if (!this.ticketId) {
      this.error.set('Invalid ticket identifier.');
      return;
    }

    this.loading.set(true);
    this.error.set(null);

    this.ticketApi
      .getById(this.ticketId)
      .pipe(
        takeUntilDestroyed(this.destroyRef),
        finalize(() => this.loading.set(false)),
      )
      .subscribe({
        next: (ticket) => {
          this.form.setValue({
            title: ticket.title,
            description: ticket.description,
            requesterName: ticket.requesterName,
            requesterEmail: ticket.requesterEmail,
            status: ticket.status,
            priority: ticket.priority,
            category: ticket.category,
          });
        },
        error: (failure: HttpErrorResponse) => {
          if (failure.status === 404) {
            void this.router.navigate(['/not-found'], { replaceUrl: true });
            return;
          }
          this.error.set('The requested ticket could not be loaded.');
        },
      });
  }
}
