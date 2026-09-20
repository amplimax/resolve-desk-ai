using Microsoft.EntityFrameworkCore;
using ResolveDeskAI.Api.Contracts.Tickets;
using ResolveDeskAI.Api.Data;
using ResolveDeskAI.Api.Domain.Enums;
using ResolveDeskAI.Api.Services.Tickets;
using ResolveDeskAI.Api.Domain.Entities;

namespace ResolveDeskAI.Api.Tests
{
    public sealed class SupportTicketServiceTests
    {
        [Fact]
        public async Task CreateAsync_PersistsNormalizedTicket()
        {
            var options =
                new DbContextOptionsBuilder<ResolveDeskDbContext>()
                    .UseInMemoryDatabase(
                        $"ResolveDeskTests-{Guid.NewGuid()}")
                    .Options;

            await using var dbContext =
                new ResolveDeskDbContext(options);

            await dbContext.Database.EnsureCreatedAsync();

            var service = new SupportTicketService(dbContext);

            var request = new CreateSupportTicketRequest
            {
                Title = "  Login problem  ",
                Description =
                    "  The customer cannot access the portal.  ",
                RequesterName = "  Laura Bianchi  ",
                RequesterEmail = "  LAURA.BIANCHI@EXAMPLE.COM  "
            };

            var result = await service.CreateAsync(
                request,
                CancellationToken.None);

            Assert.Equal("Login problem", result.Title);
            Assert.Equal(
                "The customer cannot access the portal.",
                result.Description);
            Assert.Equal("Laura Bianchi", result.RequesterName);
            Assert.Equal(
                "laura.bianchi@example.com",
                result.RequesterEmail);
            Assert.Equal(TicketStatus.Open, result.Status);
            Assert.Equal(TicketPriority.Medium, result.Priority);
            Assert.Equal(
                TicketCategory.Uncategorized,
                result.Category);
            Assert.Equal(
                1,
                await dbContext.SupportTickets.CountAsync());
        }

        [Fact]
        public async Task UpdateAsync_ResolvesExistingTicket()
        {
            var options =
                new DbContextOptionsBuilder<ResolveDeskDbContext>()
                    .UseInMemoryDatabase(
                        $"ResolveDeskTests-{Guid.NewGuid()}")
                    .Options;

            await using var dbContext =
                new ResolveDeskDbContext(options);

            await dbContext.Database.EnsureCreatedAsync();

            var service = new SupportTicketService(dbContext);

            var created = await service.CreateAsync(
                new CreateSupportTicketRequest
                {
                    Title = "Login problem",
                    Description =
                        "The customer cannot access the portal.",
                    RequesterName = "Laura Bianchi",
                    RequesterEmail =
                        "laura.bianchi@example.com"
                },
                CancellationToken.None);

            var updated = await service.UpdateAsync(
                created.Id,
                new UpdateSupportTicketRequest
                {
                    Title = created.Title,
                    Description = created.Description,
                    RequesterName = created.RequesterName,
                    RequesterEmail = created.RequesterEmail,
                    Status = TicketStatus.Resolved,
                    Priority = TicketPriority.High,
                    Category = TicketCategory.AccountAccess
                },
                CancellationToken.None);

            Assert.NotNull(updated);
            Assert.Equal(TicketStatus.Resolved, updated.Status);
            Assert.Equal(TicketPriority.High, updated.Priority);
            Assert.Equal(
                TicketCategory.AccountAccess,
                updated.Category);
            Assert.NotNull(updated.ResolvedAtUtc);
            Assert.True(
                updated.UpdatedAtUtc >= created.UpdatedAtUtc);
        }

        [Fact]
        public async Task UpdateAsync_RemovesStaleAnalysisWhenContentChanges()
        {
            var options =
                new DbContextOptionsBuilder<ResolveDeskDbContext>()
                    .UseInMemoryDatabase(
                        $"ResolveDeskTests-{Guid.NewGuid()}")
                    .Options;

            await using var dbContext =
                new ResolveDeskDbContext(options);

            await dbContext.Database.EnsureCreatedAsync();

            var service = new SupportTicketService(dbContext);

            var created = await service.CreateAsync(
                new CreateSupportTicketRequest
                {
                    Title = "Login problem",
                    Description =
                        "The customer cannot access the portal.",
                    RequesterName = "Laura Bianchi",
                    RequesterEmail =
                        "laura.bianchi@example.com"
                },
                CancellationToken.None);

            dbContext.TicketAiAnalyses.Add(
                new TicketAiAnalysis
                {
                    SupportTicketId = created.Id,
                    Summary = "The requester cannot access the portal.",
                    SuggestedCategory =
                        TicketCategory.AccountAccess,
                    SuggestedPriority = TicketPriority.High,
                    SuggestedResponse =
                        "We are investigating the authentication issue.",
                    MissingInformationJson = "[]",
                    Provider = "MockAI",
                    Model = "deterministic-v1",
                    AnalyzedAtUtc = DateTimeOffset.UtcNow
                });

            await dbContext.SaveChangesAsync();

            var updated = await service.UpdateAsync(
                created.Id,
                new UpdateSupportTicketRequest
                {
                    Title = "Login problem after password reset",
                    Description = created.Description,
                    RequesterName = created.RequesterName,
                    RequesterEmail = created.RequesterEmail,
                    Status = created.Status,
                    Priority = created.Priority,
                    Category = created.Category
                },
                CancellationToken.None);

            Assert.NotNull(updated);
            Assert.Empty(
                await dbContext.TicketAiAnalyses.ToListAsync());
        }

        [Fact]
        public async Task DeleteAsync_RemovesExistingTicket()
        {
            var options =
                new DbContextOptionsBuilder<ResolveDeskDbContext>()
                    .UseInMemoryDatabase(
                        $"ResolveDeskTests-{Guid.NewGuid()}")
                    .Options;

            await using var dbContext =
                new ResolveDeskDbContext(options);

            await dbContext.Database.EnsureCreatedAsync();

            var service = new SupportTicketService(dbContext);

            var created = await service.CreateAsync(
                new CreateSupportTicketRequest
                {
                    Title = "Login problem",
                    Description =
                        "The customer cannot access the portal.",
                    RequesterName = "Laura Bianchi",
                    RequesterEmail =
                        "laura.bianchi@example.com"
                },
                CancellationToken.None);

            var deleted = await service.DeleteAsync(
                created.Id,
                CancellationToken.None);

            var deletedAgain = await service.DeleteAsync(
                created.Id,
                CancellationToken.None);

            Assert.True(deleted);
            Assert.False(deletedAgain);
            Assert.Empty(await dbContext.SupportTickets.ToListAsync());
        }

        [Fact]
        public async Task GetAllAsync_AppliesCombinedFilters()
        {
            var options =
                new DbContextOptionsBuilder<ResolveDeskDbContext>()
                    .UseInMemoryDatabase(
                        $"ResolveDeskTests-{Guid.NewGuid()}")
                    .Options;

            await using var dbContext =
                new ResolveDeskDbContext(options);

            await dbContext.Database.EnsureCreatedAsync();

            var service = new SupportTicketService(dbContext);

            var portalTicket = await service.CreateAsync(
                new CreateSupportTicketRequest
                {
                    Title = "Portal timeout",
                    Description =
                        "The customer portal stops responding after login.",
                    RequesterName = "Laura Bianchi",
                    RequesterEmail = "laura.bianchi@example.com"
                },
                CancellationToken.None);

            await service.CreateAsync(
                new CreateSupportTicketRequest
                {
                    Title = "Invoice correction",
                    Description =
                        "The customer requested an invoice correction.",
                    RequesterName = "Marco Rossi",
                    RequesterEmail = "marco.rossi@example.com"
                },
                CancellationToken.None);

            await service.UpdateAsync(
                portalTicket.Id,
                new UpdateSupportTicketRequest
                {
                    Title = portalTicket.Title,
                    Description = portalTicket.Description,
                    RequesterName = portalTicket.RequesterName,
                    RequesterEmail = portalTicket.RequesterEmail,
                    Status = TicketStatus.InProgress,
                    Priority = TicketPriority.High,
                    Category = TicketCategory.Technical
                },
                CancellationToken.None);

            var result = await service.GetAllAsync(
                new SupportTicketQueryParameters
                {
                    Search = "Portal",
                    Status = TicketStatus.InProgress,
                    Priority = TicketPriority.High,
                    Category = TicketCategory.Technical
                },
                CancellationToken.None);

            var ticket = Assert.Single(result);

            Assert.Equal(portalTicket.Id, ticket.Id);
            Assert.Equal("Portal timeout", ticket.Title);
        }
    }
}