using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ResolveDeskAI.Api.Contracts.Tickets;
using ResolveDeskAI.Api.Data;
using ResolveDeskAI.Api.Domain.Enums;
using ResolveDeskAI.Api.Services.Tickets;

namespace ResolveDeskAI.Api.Tests
{
    public sealed class SupportTicketServiceTests
    {
        [Fact]
        public async Task CreateAsync_PersistsNormalizedTicket()
        {
            await using var connection =
                new SqliteConnection("Data Source=:memory:");

            await connection.OpenAsync();

            var options =
                new DbContextOptionsBuilder<ResolveDeskDbContext>()
                    .UseSqlite(connection)
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
            await using var connection =
                new SqliteConnection("Data Source=:memory:");

            await connection.OpenAsync();

            var options =
                new DbContextOptionsBuilder<ResolveDeskDbContext>()
                    .UseSqlite(connection)
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
        public async Task DeleteAsync_RemovesExistingTicket()
        {
            await using var connection =
                new SqliteConnection("Data Source=:memory:");

            await connection.OpenAsync();

            var options =
                new DbContextOptionsBuilder<ResolveDeskDbContext>()
                    .UseSqlite(connection)
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
    }
}