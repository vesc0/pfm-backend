using AutoMapper;
using MediatR;
using PFM.Application.Common.Pagination;
using PFM.Application.Dtos;
using PFM.Domain.Enums;
using PFM.Domain.Interfaces;

namespace PFM.Application.Queries.Transaction
{
    public class ListTransactionsQueryHandler : IRequestHandler<ListTransactionsQuery, PagedResult<TransactionDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public ListTransactionsQueryHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<PagedResult<TransactionDto>> Handle(ListTransactionsQuery request, CancellationToken cancellationToken)
        {

            // Parse the kinds CSV string into a list of TransactionKind enums
            var kindsList = request.Kinds?
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Select(s => Enum.TryParse<TransactionKindEnum>(s, true, out var v) ? (TransactionKindEnum?)v : null)
                .Where(v => v.HasValue)
                .Select(v => v!.Value)
                .ToList();

            // Query the repository
            var (entities, total) = await _uow.Transactions.ListAsync(
                request.StartDate,
                request.EndDate,
                kindsList,
                request.Page,
                request.PageSize,
                request.SortBy,
                request.SortOrder,
                cancellationToken);

            // Map domain entities to DTOs
            var dtos = _mapper.Map<List<TransactionDto>>(entities);

            // Wrap in PagedResult
            return new PagedResult<TransactionDto>
            {
                Items = dtos,
                Page = request.Page,
                PageSize = request.PageSize,
                SortBy = request.SortBy,
                SortOrder = request.SortOrder,
                TotalCount = total
            };
        }
    }
}
