using ResolveDeskAI.Api.Contracts.Tickets;

namespace ResolveDeskAI.Api.Services.Tickets
{
    public interface ISupportTicketService
    {
        Task<IReadOnlyList<SupportTicketResponse>> GetAllAsync(
            CancellationToken cancellationToken);

        Task<SupportTicketResponse?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken);

        Task<SupportTicketResponse> CreateAsync(
            CreateSupportTicketRequest request,
            CancellationToken cancellationToken);

        Task<SupportTicketResponse?> UpdateAsync(
            Guid id,
            UpdateSupportTicketRequest request,
            CancellationToken cancellationToken);

        Task<bool> DeleteAsync(
            Guid id,
            CancellationToken cancellationToken);
    }
}