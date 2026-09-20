using Microsoft.EntityFrameworkCore;
using ResolveDeskAI.Api.Data;
using ResolveDeskAI.Api.Domain.Entities;
using ResolveDeskAI.Api.Services.AI;
using ResolveDeskAI.Api.Services.Tickets;

namespace ResolveDeskAI.Api.Tests
{
    public sealed class TicketAnalysisServiceTests
    {
        [Fact]
        public async Task AnalyzeAsync_PersistsSingleAnalysisPerTicket()
        {
            var options =
                new DbContextOptionsBuilder<ResolveDeskDbContext>()
                    .UseInMemoryDatabase(
                        $"ResolveDeskTests-{Guid.NewGuid()}")
                    .Options;

            await using var dbContext =
                new ResolveDeskDbContext(options);

            await dbContext.Database.EnsureCreatedAsync();

            var ticket = new SupportTicket
            {
                Title = "Unable to access customer portal",
                Description =
                    "The customer receives an authentication "
                    + "error after entering valid credentials.",
                RequesterName = "Laura Bianchi",
                RequesterEmail = "laura.bianchi@example.com"
            };

            dbContext.SupportTickets.Add(ticket);
            await dbContext.SaveChangesAsync();

            var service = new TicketAnalysisService(
                dbContext,
                new MockTicketAnalyzer());

            var analysisBeforeCreation = await service.GetAsync(
                ticket.Id,
                CancellationToken.None);

            var firstAnalysis = await service.AnalyzeAsync(
                ticket.Id,
                CancellationToken.None);

            var secondAnalysis = await service.AnalyzeAsync(
                ticket.Id,
                CancellationToken.None);

            var persistedAnalysis = await service.GetAsync(
                ticket.Id,
                CancellationToken.None);

            Assert.Null(analysisBeforeCreation);
            Assert.NotNull(firstAnalysis);
            Assert.NotNull(secondAnalysis);
            Assert.NotNull(persistedAnalysis);
            Assert.Equal("MockAI", persistedAnalysis.Provider);
            Assert.Equal(
                "deterministic-v1",
                persistedAnalysis.Model);
            Assert.Equal(
                1,
                await dbContext.TicketAiAnalyses.CountAsync());
        }
    }
}