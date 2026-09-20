import { TicketCategory, TicketPriority } from './support-ticket';

export interface TicketAiAnalysis {
  ticketId: string;
  summary: string;
  suggestedCategory: TicketCategory;
  suggestedPriority: TicketPriority;
  suggestedResponse: string;
  missingInformation: string[];
  provider: string;
  model: string;
  analyzedAtUtc: string;
}
