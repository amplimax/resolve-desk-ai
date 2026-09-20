using Microsoft.EntityFrameworkCore;
using ResolveDeskAI.Api.Contracts.Tickets;
using ResolveDeskAI.Api.Data;
using ResolveDeskAI.Api.Domain.Entities;
using ResolveDeskAI.Api.Domain.Enums;

namespace ResolveDeskAI.Api.Services.Tickets
{
    public sealed class SupportTicketService : ISupportTicketService
    {
        private readonly ResolveDeskDbContext _dbContext;

        public SupportTicketService(ResolveDeskDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<SupportTicketResponse>> GetAllAsync(
            CancellationToken cancellationToken)
        {
            return await _dbContext.SupportTickets
                .AsNoTracking()
                .OrderByDescending(ticket => ticket.CreatedAtUtc)
                .Select(ticket => MapToResponse(ticket))
                .ToListAsync(cancellationToken);
        }

        public async Task<SupportTicketResponse?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            var ticket = await _dbContext.SupportTickets
                .AsNoTracking()
                .SingleOrDefaultAsync(
                    ticket => ticket.Id == id,
                    cancellationToken);

            return ticket is null ? null : MapToResponse(ticket);
        }

        public async Task<SupportTicketResponse> CreateAsync(
            CreateSupportTicketRequest request,
            CancellationToken cancellationToken)
        {
            var now = DateTimeOffset.UtcNow;

            var ticket = new SupportTicket
            {
                Title = request.Title.Trim(),
                Description = request.Description.Trim(),
                RequesterName = request.RequesterName.Trim(),
                RequesterEmail = request.RequesterEmail
                    .Trim()
                    .ToLowerInvariant(),
                Status = TicketStatus.Open,
                Priority = TicketPriority.Medium,
                Category = TicketCategory.Uncategorized,
                CreatedAtUtc = now,
                UpdatedAtUtc = now
            };

            _dbContext.SupportTickets.Add(ticket);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return MapToResponse(ticket);
        }

        public async Task<SupportTicketResponse?> UpdateAsync(
            Guid id,
            UpdateSupportTicketRequest request,
            CancellationToken cancellationToken)
        {
            var ticket = await _dbContext.SupportTickets
                .SingleOrDefaultAsync(
                    ticket => ticket.Id == id,
                    cancellationToken);

            if (ticket is null)
            {
                return null;
            }

            var now = DateTimeOffset.UtcNow;

            ticket.Title = request.Title.Trim();
            ticket.Description = request.Description.Trim();
            ticket.RequesterName = request.RequesterName.Trim();
            ticket.RequesterEmail = request.RequesterEmail
                .Trim()
                .ToLowerInvariant();
            ticket.Status = request.Status;
            ticket.Priority = request.Priority;
            ticket.Category = request.Category;
            ticket.UpdatedAtUtc = now;
            ticket.ResolvedAtUtc =
                request.Status == TicketStatus.Resolved
                    ? ticket.ResolvedAtUtc ?? now
                    : null;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return MapToResponse(ticket);
        }

        public async Task<bool> DeleteAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            var ticket = await _dbContext.SupportTickets
                .SingleOrDefaultAsync(
                    ticket => ticket.Id == id,
                    cancellationToken);

            if (ticket is null)
            {
                return false;
            }

            _dbContext.SupportTickets.Remove(ticket);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }

        private static SupportTicketResponse MapToResponse(
            SupportTicket ticket)
        {
            return new SupportTicketResponse(
                ticket.Id,
                ticket.Title,
                ticket.Description,
                ticket.RequesterName,
                ticket.RequesterEmail,
                ticket.Status,
                ticket.Priority,
                ticket.Category,
                ticket.CreatedAtUtc,
                ticket.UpdatedAtUtc,
                ticket.ResolvedAtUtc);
        }
    }
}