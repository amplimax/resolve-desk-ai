export type TicketStatus = 'Open' | 'InProgress' | 'Resolved';

export type TicketPriority = 'Low' | 'Medium' | 'High' | 'Critical';

export type TicketCategory =
  'Uncategorized' | 'Technical' | 'AccountAccess' | 'Billing' | 'FeatureRequest' | 'Other';

export interface SupportTicket {
  id: string;
  title: string;
  description: string;
  requesterName: string;
  requesterEmail: string;
  status: TicketStatus;
  priority: TicketPriority;
  category: TicketCategory;
  createdAtUtc: string;
  updatedAtUtc: string;
  resolvedAtUtc: string | null;
}

export interface SupportTicketQuery {
  search?: string;
  status?: TicketStatus;
  priority?: TicketPriority;
  category?: TicketCategory;
}
