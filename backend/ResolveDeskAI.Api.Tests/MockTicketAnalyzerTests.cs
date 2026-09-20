using ResolveDeskAI.Api.Domain.Enums;
using ResolveDeskAI.Api.Services.AI;

namespace ResolveDeskAI.Api.Tests
{
    public sealed class MockTicketAnalyzerTests
    {
        [Fact]
        public async Task AnalyzeAsync_ReturnsStructuredAnalysis()
        {
            var analyzer = new MockTicketAnalyzer();

            var result = await analyzer.AnalyzeAsync(
                new TicketAnalysisInput(
                    "Unable to access customer portal",
                    "The customer receives an authentication "
                    + "error after entering valid credentials."),
                CancellationToken.None);

            Assert.Equal(
                TicketCategory.AccountAccess,
                result.SuggestedCategory);
            Assert.Equal(
                TicketPriority.High,
                result.SuggestedPriority);
            Assert.Equal("MockAI", result.Provider);
            Assert.Equal("deterministic-v1", result.Model);
            Assert.NotEmpty(result.Summary);
            Assert.NotEmpty(result.SuggestedResponse);
            Assert.NotEmpty(result.MissingInformation);
        }
    }
}