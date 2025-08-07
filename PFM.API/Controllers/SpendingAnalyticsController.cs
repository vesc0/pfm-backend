using MediatR;
using Microsoft.AspNetCore.Mvc;
using PFM.Application.Dtos;
using PFM.Application.Queries.SpendingAnalytics;

namespace PFM.API.Controllers
{
    [ApiController]
    [Route("")]
    public class AnalyticsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AnalyticsController(IMediator mediator)
            => _mediator = mediator;

        [HttpGet("spending-analytics")]
        [ProducesResponseType(typeof(SpendingsByCategoryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetSpendingAnalytics(
            [FromQuery(Name = "catcode")] string? category,
            [FromQuery(Name = "start-date")] DateTime? startDate,
            [FromQuery(Name = "end-date")] DateTime? endDate,
            [FromQuery(Name = "direction")] string? direction)
        {
            var dto = await _mediator.Send(new SpendingAnalyticsQuery
            {
                Category = category,
                StartDate = startDate,
                EndDate = endDate,
                Direction = direction
            });
            return Ok(dto);
        }

    }
}
