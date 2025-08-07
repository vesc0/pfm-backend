using MediatR;
using Microsoft.AspNetCore.Mvc;
using PFM.Application.Commands.Category;
using PFM.Application.Queries.Category;

namespace PFM.API.Controllers
{
    [ApiController]
    [Route("categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoriesController(IMediator mediator)
            => _mediator = mediator;

        [HttpPost("import")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Import(IFormFile file)
        {
            if (file is null)
                return BadRequest(new { error = "A CSV file must be provided." });

            if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                return BadRequest(new { error = "Only .csv files are supported." });

            await using var stream = file.OpenReadStream();
            var cmd = new ImportCategoriesCommand(stream);
            await _mediator.Send(cmd);

            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories([FromQuery(Name = "parent-id")] string? parentId)
        {
            var result = await _mediator.Send(new GetCategoriesQuery { ParentId = parentId });
            return Ok(new { items = result });
        }
    }
}
