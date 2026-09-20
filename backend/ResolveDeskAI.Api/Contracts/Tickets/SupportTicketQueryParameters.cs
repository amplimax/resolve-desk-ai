using System.ComponentModel.DataAnnotations;
using ResolveDeskAI.Api.Domain.Enums;

namespace ResolveDeskAI.Api.Contracts.Tickets
{
    public sealed class SupportTicketQueryParameters
    {
        [StringLength(200)]
        public string? Search { get; init; }

        public TicketStatus? Status { get; init; }

        public TicketPriority? Priority { get; init; }

        public TicketCategory? Category { get; init; }
    }
}