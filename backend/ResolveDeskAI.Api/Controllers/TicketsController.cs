using Microsoft.AspNetCore.Mvc;
using ResolveDeskAI.Api.Contracts.Tickets;
using ResolveDeskAI.Api.Services.Tickets;

namespace ResolveDeskAI.Api.Controllers
{
    [ApiController]
    [Route("api/tickets")]
    public sealed class TicketsController : ControllerBase
    {
        private readonly ISupportTicketService _ticketService;

        public TicketsController(ISupportTicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpGet]
        [ProducesResponseType<IReadOnlyList<SupportTicketResponse>>(
            StatusCodes.Status200OK)]
        public async Task<ActionResult<
            IReadOnlyList<SupportTicketResponse>>> GetAll(
            CancellationToken cancellationToken)
        {
            var tickets = await _ticketService.GetAllAsync(
                cancellationToken);

            return Ok(tickets);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType<SupportTicketResponse>(
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SupportTicketResponse>> GetById(
            Guid id,
            CancellationToken cancellationToken)
        {
            var ticket = await _ticketService.GetByIdAsync(
                id,
                cancellationToken);

            return ticket is null
                ? NotFound()
                : Ok(ticket);
        }

        [HttpPost]
        [ProducesResponseType<SupportTicketResponse>(
            StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SupportTicketResponse>> Create(
            CreateSupportTicketRequest request,
            CancellationToken cancellationToken)
        {
            var ticket = await _ticketService.CreateAsync(
                request,
                cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = ticket.Id },
                ticket);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType<SupportTicketResponse>(
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SupportTicketResponse>> Update(
            Guid id,
            UpdateSupportTicketRequest request,
            CancellationToken cancellationToken)
        {
            var ticket = await _ticketService.UpdateAsync(
                id,
                request,
                cancellationToken);

            return ticket is null
                ? NotFound()
                : Ok(ticket);
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(
            Guid id,
            CancellationToken cancellationToken)
        {
            var deleted = await _ticketService.DeleteAsync(
                id,
                cancellationToken);

            return deleted
                ? NoContent()
                : NotFound();
        }
    }
}