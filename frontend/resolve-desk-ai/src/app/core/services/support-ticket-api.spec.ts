import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';

import { SupportTicket } from '../models/support-ticket';
import { SupportTicketApi } from './support-ticket-api';

describe('SupportTicketApi', () => {
  let service: SupportTicketApi;
  let httpTesting: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });

    service = TestBed.inject(SupportTicketApi);
    httpTesting = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpTesting.verify();
  });

  it('loads support tickets from the API', () => {
    const tickets: SupportTicket[] = [
      {
        id: 'ticket-1',
        title: 'Unable to access customer portal',
        description: 'Authentication error.',
        requesterName: 'Laura Bianchi',
        requesterEmail: 'laura.bianchi@example.com',
        status: 'Open',
        priority: 'Medium',
        category: 'Uncategorized',
        createdAtUtc: '2026-09-20T10:46:56+00:00',
        updatedAtUtc: '2026-09-20T10:46:56+00:00',
        resolvedAtUtc: null,
      },
    ];

    service.getAll().subscribe((result) => {
      expect(result).toEqual(tickets);
    });

    const request = httpTesting.expectOne('/api/tickets');

    expect(request.request.method).toBe('GET');

    request.flush(tickets);
  });

  it('creates a support ticket', () => {
    const requestBody = {
      title: 'Unable to access customer portal',
      description: 'Authentication fails with valid credentials.',
      requesterName: 'Laura Bianchi',
      requesterEmail: 'laura.bianchi@example.com',
    };

    const createdTicket: SupportTicket = {
      id: 'ticket-1',
      ...requestBody,
      status: 'Open',
      priority: 'Medium',
      category: 'Uncategorized',
      createdAtUtc: '2026-09-20T10:46:56+00:00',
      updatedAtUtc: '2026-09-20T10:46:56+00:00',
      resolvedAtUtc: null,
    };

    service.create(requestBody).subscribe((result) => {
      expect(result).toEqual(createdTicket);
    });

    const request = httpTesting.expectOne('/api/tickets');

    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual(requestBody);

    request.flush(createdTicket);
  });

  it('sends combined ticket filters as query parameters', () => {
    service
      .getAll({
        search: ' portal ',
        status: 'Open',
        priority: 'High',
        category: 'AccountAccess',
      })
      .subscribe((result) => {
        expect(result).toEqual([]);
      });

    const request = httpTesting.expectOne((candidate) => candidate.url === '/api/tickets');

    expect(request.request.method).toBe('GET');
    expect(request.request.params.get('search')).toBe('portal');
    expect(request.request.params.get('status')).toBe('Open');
    expect(request.request.params.get('priority')).toBe('High');
    expect(request.request.params.get('category')).toBe('AccountAccess');

    request.flush([]);
  });

  it('updates a support ticket', () => {
    const requestBody = {
      title: 'Unable to access customer portal',
      description: 'Authentication fails with valid credentials.',
      requesterName: 'Laura Bianchi',
      requesterEmail: 'laura.bianchi@example.com',
      status: 'InProgress' as const,
      priority: 'High' as const,
      category: 'AccountAccess' as const,
    };

    const updatedTicket: SupportTicket = {
      id: 'ticket-1',
      ...requestBody,
      createdAtUtc: '2026-09-20T10:46:56+00:00',
      updatedAtUtc: '2026-09-20T14:00:00+00:00',
      resolvedAtUtc: null,
    };

    service.update('ticket-1', requestBody).subscribe((result) => {
      expect(result).toEqual(updatedTicket);
    });

    const request = httpTesting.expectOne('/api/tickets/ticket-1');

    expect(request.request.method).toBe('PUT');
    expect(request.request.body).toEqual(requestBody);

    request.flush(updatedTicket);
  });
});
