using MediatR;
using Microsoft.AspNetCore.Mvc;
using PFM.API.Contracts;
using PFM.Application.Commands.Transaction;
using PFM.Application.Queries.Transaction;

namespace PFM.API.Controllers
{
    [ApiController]
    [Route("transactions")]
    public class TransactionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TransactionsController(IMediator mediator)
            => _mediator = mediator;

        [HttpPost("import")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(440)]  // Business‐policy violations (DomainException)
        public async Task<IActionResult> Import([FromForm] ImportFileRequest form)
        {
            await using var stream = form.File!.OpenReadStream();
            await _mediator.Send(new ImportTransactionsCommand(stream));

            return Ok(new { message = "Transactions imported successfully." });
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> List(
        [FromQuery(Name = "start-date")] DateTime? startDate,
        [FromQuery(Name = "end-date")] DateTime? endDate,
        [FromQuery(Name = "transaction-kind")] string? kinds,
        [FromQuery(Name = "page")] int page = 1,
        [FromQuery(Name = "page-size")] int pageSize = 10,
        [FromQuery(Name = "sort-by")] string? sortBy = "date",
        [FromQuery(Name = "sort-order")] string? sortOrder = "asc")
        {

            var query = new ListTransactionsQuery(startDate, endDate, kinds, sortBy, sortOrder, page, pageSize);
            var result = await _mediator.Send(query);

            return Ok(result);
        }


        [HttpPost("{id}/categorize")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(440)] // OAS3: business policy error
        public async Task<IActionResult> Categorize(
            [FromRoute] string id,
            [FromBody] CategorizeTransactionRequest body)
        {
            await _mediator.Send(new CategorizeTransactionCommand(id, body.CatCode));
            return Ok(new { message = "Transaction categorized." });
        }

        [HttpPost("{id}/split")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(440)] // OAS3: business policy error
        public async Task<IActionResult> Split(string id, [FromBody] SplitTransactionRequest request)
        {
            await _mediator.Send(new SplitTransactionCommand { TransactionId = id, Splits = request.Splits });
            return Ok(new { message = "Transaction splitted." });
        }

        [HttpPost("auto-categorize")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(440)]
        public async Task<IActionResult> AutoCategorize()
        {
            var result = await _mediator.Send(new AutoCategorizeTransactionsCommand());
            return Ok(result);
        }

    }
}
