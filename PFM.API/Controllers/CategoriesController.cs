using MediatR;
using Microsoft.AspNetCore.Mvc;
using PFM.API.Contracts;
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(440)]  // Business‐policy violations (DomainException)
        public async Task<IActionResult> Import([FromForm] ImportFileDto form)
        {
            await using var stream = form.File!.OpenReadStream();
            await _mediator.Send(new ImportCategoriesCommand(stream));

            return Ok("Categories imported successfully.");
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(440)]  // Business‐policy violations (DomainException)
        public async Task<IActionResult> GetCategories([FromQuery(Name = "parent-id")] string? parentId)
        {
            var result = await _mediator.Send(new GetCategoriesQuery { ParentId = parentId });
            return Ok(new { items = result });
        }
    }
}
