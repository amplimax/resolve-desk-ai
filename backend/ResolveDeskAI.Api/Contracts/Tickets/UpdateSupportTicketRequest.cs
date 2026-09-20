using System.ComponentModel.DataAnnotations;
using ResolveDeskAI.Api.Domain.Enums;

namespace ResolveDeskAI.Api.Contracts.Tickets
{
    public sealed class UpdateSupportTicketRequest
    {
        [Required]
        [StringLength(200, MinimumLength = 3)]
        public string Title { get; init; } = string.Empty;

        [Required]
        [StringLength(4000, MinimumLength = 10)]
        public string Description { get; init; } = string.Empty;

        [Required]
        [StringLength(120, MinimumLength = 2)]
        public string RequesterName { get; init; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(254)]
        public string RequesterEmail { get; init; } = string.Empty;

        [EnumDataType(typeof(TicketStatus))]
        public TicketStatus Status { get; init; }

        [EnumDataType(typeof(TicketPriority))]
        public TicketPriority Priority { get; init; }

        [EnumDataType(typeof(TicketCategory))]
        public TicketCategory Category { get; init; }
    }
}