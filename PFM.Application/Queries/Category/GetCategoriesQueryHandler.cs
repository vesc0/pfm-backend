using AutoMapper;
using MediatR;
using PFM.Domain.Interfaces;
using PFM.Application.Dtos;
using PFM.Domain.Exceptions;

namespace PFM.Application.Queries.Category
{
    public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, List<CategoryDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetCategoriesQueryHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<List<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(request.ParentId))
            {
                var exists = await _uow.Categories.ExistsAsync(new[] { request.ParentId }, cancellationToken);
                if (!exists)
                {
                    throw new BusinessRuleException(
                        "parent-category-not-found",
                        "Parent category not found",
                        $"No category with code '{request.ParentId}' exists."
                    );
                }
            }
            var categories = await _uow.Categories.ListAsync(request.ParentId, cancellationToken);
            return _mapper.Map<List<CategoryDto>>(categories);
        }
    }
}