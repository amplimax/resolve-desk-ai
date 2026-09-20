using ResolveDeskAI.Api.Domain.Enums;

namespace ResolveDeskAI.Api.Contracts.Tickets
{
    public sealed record TicketAiAnalysisResponse(
        Guid TicketId,
        string Summary,
        TicketCategory SuggestedCategory,
        TicketPriority SuggestedPriority,
        string SuggestedResponse,
        IReadOnlyList<string> MissingInformation,
        string Provider,
        string Model,
        DateTimeOffset AnalyzedAtUtc);
}