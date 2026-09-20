namespace ResolveDeskAI.Api.Services.AI
{
    public interface ITicketAnalyzer
    {
        Task<TicketAnalysisResult> AnalyzeAsync(
            TicketAnalysisInput input,
            CancellationToken cancellationToken);
    }
}