using Microsoft.AspNetCore.Mvc;
using ResolveDeskAI.Api.Contracts.Tickets;
using ResolveDeskAI.Api.Services.Tickets;

namespace ResolveDeskAI.Api.Controllers
{
    [ApiController]
    [Route("api/tickets/{ticketId:guid}/analysis")]
    public sealed class TicketAnalysisController : ControllerBase
    {
        private readonly ITicketAnalysisService _analysisService;

        public TicketAnalysisController(
            ITicketAnalysisService analysisService)
        {
            _analysisService = analysisService;
        }

        [HttpGet]
        [ProducesResponseType<TicketAiAnalysisResponse>(
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<
            TicketAiAnalysisResponse>> Get(
            Guid ticketId,
            CancellationToken cancellationToken)
        {
            var analysis = await _analysisService.GetAsync(
                ticketId,
                cancellationToken);

            return analysis is null
                ? NotFound()
                : Ok(analysis);
        }

        [HttpPost]
        [ProducesResponseType<TicketAiAnalysisResponse>(
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<
            TicketAiAnalysisResponse>> Analyze(
            Guid ticketId,
            CancellationToken cancellationToken)
        {
            var analysis = await _analysisService.AnalyzeAsync(
                ticketId,
                cancellationToken);

            return analysis is null
                ? NotFound()
                : Ok(analysis);
        }
    }
}