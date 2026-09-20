using ResolveDeskAI.Api.Domain.Enums;

namespace ResolveDeskAI.Api.Contracts.Tickets
{
    public sealed record SupportTicketResponse(
        Guid Id,
        string Title,
        string Description,
        string RequesterName,
        string RequesterEmail,
        TicketStatus Status,
        TicketPriority Priority,
        TicketCategory Category,
        DateTimeOffset CreatedAtUtc,
        DateTimeOffset UpdatedAtUtc,
        DateTimeOffset? ResolvedAtUtc);
}