using ResolveDeskAI.Api.Contracts.Tickets;

namespace ResolveDeskAI.Api.Services.Tickets
{
    public interface ITicketAnalysisService
    {
        Task<TicketAiAnalysisResponse?> GetAsync(
            Guid ticketId,
            CancellationToken cancellationToken);

        Task<TicketAiAnalysisResponse?> AnalyzeAsync(
            Guid ticketId,
            CancellationToken cancellationToken);
    }
}