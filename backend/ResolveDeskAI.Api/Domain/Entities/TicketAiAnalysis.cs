using ResolveDeskAI.Api.Domain.Enums;

namespace ResolveDeskAI.Api.Domain.Entities
{
    public sealed class TicketAiAnalysis
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid SupportTicketId { get; set; }

        public string Summary { get; set; } = string.Empty;

        public TicketCategory SuggestedCategory { get; set; }

        public TicketPriority SuggestedPriority { get; set; }

        public string SuggestedResponse { get; set; } = string.Empty;

        public string MissingInformationJson { get; set; } = "[]";

        public string Provider { get; set; } = string.Empty;

        public string Model { get; set; } = string.Empty;

        public DateTimeOffset AnalyzedAtUtc { get; set; } =
            DateTimeOffset.UtcNow;

        public SupportTicket SupportTicket { get; set; } = null!;
    }
}