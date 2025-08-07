using MediatR;
using PFM.Application.Dtos;

namespace PFM.Application.Queries.Category
{
    public class GetCategoriesQuery : IRequest<List<CategoryDto>>
    {
        public string? ParentId { get; set; }
    }
}