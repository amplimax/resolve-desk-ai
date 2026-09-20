using ResolveDeskAI.Api.Domain.Enums;

namespace ResolveDeskAI.Api.Services.AI
{
    public sealed record TicketAnalysisResult(
        string Summary,
        TicketCategory SuggestedCategory,
        TicketPriority SuggestedPriority,
        string SuggestedResponse,
        IReadOnlyList<string> MissingInformation,
        string Provider,
        string Model);
}