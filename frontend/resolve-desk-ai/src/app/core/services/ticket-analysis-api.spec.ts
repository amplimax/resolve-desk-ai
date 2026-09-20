import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';

import { TicketAiAnalysis } from '../models/ticket-ai-analysis';
import { TicketAnalysisApi } from './ticket-analysis-api';

describe('TicketAnalysisApi', () => {
  let service: TicketAnalysisApi;
  let httpTesting: HttpTestingController;

  const analysis: TicketAiAnalysis = {
    ticketId: 'ticket-1',
    summary: 'The requester cannot access the customer portal.',
    suggestedCategory: 'AccountAccess',
    suggestedPriority: 'High',
    suggestedResponse: 'We are reviewing the authentication issue.',
    missingInformation: ['Browser and operating system'],
    provider: 'MockAI',
    model: 'deterministic-v1',
    analyzedAtUtc: '2026-09-20T12:00:00+00:00',
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });

    service = TestBed.inject(TicketAnalysisApi);
    httpTesting = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpTesting.verify();
  });

  it('returns null when no persisted analysis exists', () => {
    service.get('ticket-1').subscribe((result) => {
      expect(result).toBeNull();
    });

    const request = httpTesting.expectOne('/api/tickets/ticket-1/analysis');

    expect(request.request.method).toBe('GET');

    request.flush(null, {
      status: 404,
      statusText: 'Not Found',
    });
  });

  it('runs the ticket analysis', () => {
    service.analyze('ticket-1').subscribe((result) => {
      expect(result).toEqual(analysis);
    });

    const request = httpTesting.expectOne('/api/tickets/ticket-1/analysis');

    expect(request.request.method).toBe('POST');

    request.flush(analysis);
  });
});
