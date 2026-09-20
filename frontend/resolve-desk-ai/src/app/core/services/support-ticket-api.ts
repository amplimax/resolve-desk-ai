import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import {
  CreateSupportTicketRequest,
  SupportTicket,
  SupportTicketQuery,
} from '../models/support-ticket';

@Injectable({
  providedIn: 'root',
})
export class SupportTicketApi {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/tickets';

  getAll(query: SupportTicketQuery = {}): Observable<SupportTicket[]> {
    let params = new HttpParams();

    const search = query.search?.trim();

    if (search) {
      params = params.set('search', search);
    }

    if (query.status) {
      params = params.set('status', query.status);
    }

    if (query.priority) {
      params = params.set('priority', query.priority);
    }

    if (query.category) {
      params = params.set('category', query.category);
    }

    return this.http.get<SupportTicket[]>(this.baseUrl, { params });
  }

  create(request: CreateSupportTicketRequest): Observable<SupportTicket> {
    return this.http.post<SupportTicket>(this.baseUrl, request);
  }
}
