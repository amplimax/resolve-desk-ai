import { DatePipe } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { finalize, forkJoin } from 'rxjs';

import { SupportTicket } from '../../core/models/support-ticket';
import { TicketAiAnalysis } from '../../core/models/ticket-ai-analysis';
import { SupportTicketApi } from '../../core/services/support-ticket-api';
import { TicketAnalysisApi } from '../../core/services/ticket-analysis-api';
import { TicketDeleteDialog } from './ticket-delete-dialog';
import type { TicketDeleteDialogData } from './ticket-delete-dialog';

@Component({
  selector: 'app-ticket-detail-page',
  imports: [
    DatePipe,
    MatButtonModule,
    MatCardModule,
    MatDividerModule,
    MatProgressSpinnerModule,
    RouterLink,
    MatDialogModule,
  ],
  templateUrl: './ticket-detail-page.html',
  styleUrl: './ticket-detail-page.scss',
})
export class TicketDetailPage {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly dialog = inject(MatDialog);
  private readonly ticketApi = inject(SupportTicketApi);
  private readonly analysisApi = inject(TicketAnalysisApi);
  private readonly destroyRef = inject(DestroyRef);

  private readonly ticketId = this.route.snapshot.paramMap.get('id') ?? '';

  protected readonly ticket = signal<SupportTicket | null>(null);
  protected readonly analysis = signal<TicketAiAnalysis | null>(null);
  protected readonly loading = signal(false);
  protected readonly analyzing = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly analysisError = signal<string | null>(null);
  protected readonly deleting = signal(false);
  protected readonly deleteError = signal<string | null>(null);

  constructor() {
    this.load();
  }

  protected confirmDelete(ticket: SupportTicket): void {
    if (this.deleting()) {
      return;
    }

    const dialogReference = this.dialog.open<TicketDeleteDialog, TicketDeleteDialogData, boolean>(
      TicketDeleteDialog,
      {
        width: '440px',
        data: {
          title: ticket.title,
        },
      },
    );

    dialogReference
      .afterClosed()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((confirmed) => {
        if (confirmed) {
          this.deleteTicket();
        }
      });
  }

  protected analyze(): void {
    if (!this.ticketId || this.analyzing()) {
      return;
    }

    this.analyzing.set(true);
    this.analysisError.set(null);

    this.analysisApi
      .analyze(this.ticketId)
      .pipe(
        takeUntilDestroyed(this.destroyRef),
        finalize(() => this.analyzing.set(false)),
      )
      .subscribe({
        next: (analysis) => this.analysis.set(analysis),
        error: () => {
          this.analysisError.set('The AI analysis could not be completed. Please try again.');
        },
      });
  }

  private deleteTicket(): void {
    this.deleting.set(true);
    this.deleteError.set(null);

    this.ticketApi
      .delete(this.ticketId)
      .pipe(
        takeUntilDestroyed(this.destroyRef),
        finalize(() => this.deleting.set(false)),
      )
      .subscribe({
        next: () => void this.router.navigate(['/tickets']),
        error: () => {
          this.deleteError.set('The ticket could not be deleted. Please try again.');
        },
      });
  }

  private load(): void {
    if (!this.ticketId) {
      this.error.set('Invalid ticket identifier.');
      return;
    }

    this.loading.set(true);
    this.error.set(null);

    forkJoin({
      ticket: this.ticketApi.getById(this.ticketId),
      analysis: this.analysisApi.get(this.ticketId),
    })
      .pipe(
        takeUntilDestroyed(this.destroyRef),
        finalize(() => this.loading.set(false)),
      )
      .subscribe({
        next: ({ ticket, analysis }) => {
          this.ticket.set(ticket);
          this.analysis.set(analysis);
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
