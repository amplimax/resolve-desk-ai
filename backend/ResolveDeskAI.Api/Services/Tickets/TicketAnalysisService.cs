using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ResolveDeskAI.Api.Contracts.Tickets;
using ResolveDeskAI.Api.Data;
using ResolveDeskAI.Api.Domain.Entities;
using ResolveDeskAI.Api.Services.AI;

namespace ResolveDeskAI.Api.Services.Tickets
{
    public sealed class TicketAnalysisService :
        ITicketAnalysisService
    {
        private readonly ResolveDeskDbContext _dbContext;
        private readonly ITicketAnalyzer _ticketAnalyzer;

        public TicketAnalysisService(
            ResolveDeskDbContext dbContext,
            ITicketAnalyzer ticketAnalyzer)
        {
            _dbContext = dbContext;
            _ticketAnalyzer = ticketAnalyzer;
        }

        public async Task<TicketAiAnalysisResponse?> GetAsync(
            Guid ticketId,
            CancellationToken cancellationToken)
        {
            var analysis = await _dbContext.TicketAiAnalyses
                .AsNoTracking()
                .SingleOrDefaultAsync(
                    item => item.SupportTicketId == ticketId,
                    cancellationToken);

            return analysis is null
                ? null
                : MapToResponse(analysis);
        }

        public async Task<TicketAiAnalysisResponse?> AnalyzeAsync(
            Guid ticketId,
            CancellationToken cancellationToken)
        {
            var ticket = await _dbContext.SupportTickets
                .Include(item => item.AiAnalysis)
                .SingleOrDefaultAsync(
                    item => item.Id == ticketId,
                    cancellationToken);

            if (ticket is null)
            {
                return null;
            }

            var result = await _ticketAnalyzer.AnalyzeAsync(
                new TicketAnalysisInput(
                    ticket.Title,
                    ticket.Description),
                cancellationToken);

            var analysis = ticket.AiAnalysis;

            if (analysis is null)
            {
                analysis = new TicketAiAnalysis
                {
                    SupportTicketId = ticket.Id
                };

                ticket.AiAnalysis = analysis;
                _dbContext.TicketAiAnalyses.Add(analysis);
            }

            analysis.Summary = result.Summary;
            analysis.SuggestedCategory =
                result.SuggestedCategory;
            analysis.SuggestedPriority =
                result.SuggestedPriority;
            analysis.SuggestedResponse =
                result.SuggestedResponse;
            analysis.MissingInformationJson =
                JsonSerializer.Serialize(
                    result.MissingInformation);
            analysis.Provider = result.Provider;
            analysis.Model = result.Model;
            analysis.AnalyzedAtUtc = DateTimeOffset.UtcNow;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return MapToResponse(analysis);
        }

        private static TicketAiAnalysisResponse MapToResponse(
            TicketAiAnalysis analysis)
        {
            return new TicketAiAnalysisResponse(
                analysis.SupportTicketId,
                analysis.Summary,
                analysis.SuggestedCategory,
                analysis.SuggestedPriority,
                analysis.SuggestedResponse,
                DeserializeMissingInformation(
                    analysis.MissingInformationJson),
                analysis.Provider,
                analysis.Model,
                analysis.AnalyzedAtUtc);
        }

        private static IReadOnlyList<string>
            DeserializeMissingInformation(string json)
        {
            try
            {
                return JsonSerializer.Deserialize<List<string>>(json)
                    ?? new List<string>();
            }
            catch (JsonException)
            {
                return new List<string>();
            }
        }
    }
}