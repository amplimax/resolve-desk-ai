using ResolveDeskAI.Api.Domain.Enums;

namespace ResolveDeskAI.Api.Domain.Entities
{
    public sealed class SupportTicket
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string RequesterName { get; set; } = string.Empty;

        public string RequesterEmail { get; set; } = string.Empty;

        public TicketStatus Status { get; set; } = TicketStatus.Open;

        public TicketPriority Priority { get; set; } = TicketPriority.Medium;

        public TicketCategory Category { get; set; } =
            TicketCategory.Uncategorized;

        public DateTimeOffset CreatedAtUtc { get; set; } =
            DateTimeOffset.UtcNow;

        public DateTimeOffset UpdatedAtUtc { get; set; } =
            DateTimeOffset.UtcNow;

        public DateTimeOffset? ResolvedAtUtc { get; set; }
    }
}