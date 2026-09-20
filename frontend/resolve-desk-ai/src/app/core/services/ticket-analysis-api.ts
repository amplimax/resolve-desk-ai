import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { catchError, Observable, of, throwError } from 'rxjs';

import { TicketAiAnalysis } from '../models/ticket-ai-analysis';

@Injectable({
  providedIn: 'root',
})
export class TicketAnalysisApi {
  private readonly http = inject(HttpClient);

  get(ticketId: string): Observable<TicketAiAnalysis | null> {
    return this.http.get<TicketAiAnalysis>(this.buildUrl(ticketId)).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status === 404) {
          return of(null);
        }

        return throwError(() => error);
      }),
    );
  }

  analyze(ticketId: string): Observable<TicketAiAnalysis> {
    return this.http.post<TicketAiAnalysis>(this.buildUrl(ticketId), null);
  }

  private buildUrl(ticketId: string): string {
    return `/api/tickets/${ticketId}/analysis`;
  }
}
