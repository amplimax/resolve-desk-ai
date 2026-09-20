using System.ComponentModel.DataAnnotations;

namespace ResolveDeskAI.Api.Contracts.Tickets
{
    public sealed class CreateSupportTicketRequest
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
    }
}