using Microsoft.AspNetCore.Mvc;
using ResolveDeskAI.Api.Contracts;

namespace ResolveDeskAI.Api.Controllers
{
    [ApiController]
    [Route("health")]
    public sealed class HealthController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType<HealthResponse>(StatusCodes.Status200OK)]
        public ActionResult<HealthResponse> Get()
        {
            var response = new HealthResponse(
                Status: "Healthy",
                Service: "ResolveDeskAI.Api",
                TimestampUtc: DateTimeOffset.UtcNow);

            return Ok(response);
        }
    }
}