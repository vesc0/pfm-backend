using MediatR;
using PFM.Application.Common.Pagination;
using PFM.Application.Dtos;
using PFM.Domain.Enums;
using PFM.Domain.Interfaces;

namespace PFM.Application.Queries.Transaction
{
    public class ListTransactionsQueryHandler
        : IRequestHandler<ListTransactionsQuery, PagedResult<TransactionDto>>
    {
        private readonly ITransactionReadRepository _readRepo;

        public ListTransactionsQueryHandler(ITransactionReadRepository readRepo)
            => _readRepo = readRepo;

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
            var (entities, total) = await _readRepo.ListAsync(
                request.StartDate,
                request.EndDate,
                kindsList,
                request.Page,
                request.PageSize,
                request.SortBy,
                request.SortOrder,
                cancellationToken);

            // Map domain entities to DTOs
            var dtos = entities
                .Select(t => new TransactionDto
                {
                    Id = t.Id,
                    Date = t.Date,
                    Direction = t.Direction,
                    Amount = t.Amount,
                    BeneficiaryName = t.BeneficiaryName,
                    Description = t.Description,
                    Currency = t.Currency,
                    Mcc = (int?)t.Mcc,
                    Kind = t.Kind,
                    CatCode = t.CatCode,
                    Splits = t.Splits
                                .Select(s => new TransactionSplitDto
                                {
                                    Amount = s.Amount,
                                    CatCode = s.CatCode
                                })
                                .ToList()
                })
                .ToList();

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
