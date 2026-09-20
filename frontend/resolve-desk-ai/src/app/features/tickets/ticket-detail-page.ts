import { DatePipe } from '@angular/common';
import { Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { finalize, forkJoin } from 'rxjs';

import { SupportTicket } from '../../core/models/support-ticket';
import { TicketAiAnalysis } from '../../core/models/ticket-ai-analysis';
import { SupportTicketApi } from '../../core/services/support-ticket-api';
import { TicketAnalysisApi } from '../../core/services/ticket-analysis-api';

@Component({
  selector: 'app-ticket-detail-page',
  imports: [
    DatePipe,
    MatButtonModule,
    MatCardModule,
    MatDividerModule,
    MatProgressSpinnerModule,
    RouterLink,
  ],
  templateUrl: './ticket-detail-page.html',
  styleUrl: './ticket-detail-page.scss',
})
export class TicketDetailPage {
  private readonly route = inject(ActivatedRoute);
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

  constructor() {
    this.load();
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
        error: () => {
          this.error.set('The requested ticket could not be loaded.');
        },
      });
  }
}
