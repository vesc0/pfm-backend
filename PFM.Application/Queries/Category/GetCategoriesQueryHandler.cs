using MediatR;
using PFM.Domain.Interfaces;
using PFM.Application.Dtos;
using PFM.Domain.Exceptions;

namespace PFM.Application.Queries.Category
{
    public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, List<CategoryDto>>
    {
        private readonly ICategoryReadRepository _repo;
        public GetCategoriesQueryHandler(ICategoryReadRepository repo) => _repo = repo;

        public async Task<List<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(request.ParentId))
            {
                var exists = await _repo.ExistsAsync(new[] { request.ParentId }, cancellationToken);
                if (!exists)
                {
                    throw new BusinessRuleException(
                        "parent-category-not-found",
                        "Parent category not found",
                        $"No category with code '{request.ParentId}' exists."
                    );
                }
            }
            var categories = await _repo.ListAsync(request.ParentId, cancellationToken);
            return categories.Select(c => new CategoryDto
            {
                Code = c.Code,
                Name = c.Name,
                ParentCode = c.ParentCode
            }).ToList();
        }
    }
}