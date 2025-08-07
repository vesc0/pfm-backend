using MediatR;
using PFM.Application.Common.Pagination;
using PFM.Application.Dtos;

namespace PFM.Application.Queries.Transaction
{
    public class ListTransactionsQuery : IRequest<PagedResult<TransactionDto>>
    {
        public DateTime? StartDate { get; }
        public DateTime? EndDate { get; }
        public string? Kinds { get; }
        public int Page { get; }
        public int PageSize { get; }
        public string? SortBy { get; }
        public string? SortOrder { get; }

        public ListTransactionsQuery(
            DateTime? startDate,
            DateTime? endDate,
            string? kinds,
            string? sortBy,
            string? sortOrder = "asc",
            int page = 1,
            int pageSize = 10
        )
        {
            StartDate = startDate;
            EndDate = endDate;
            Kinds = kinds;
            Page = page;
            PageSize = pageSize;
            SortBy = sortBy;
            SortOrder = sortOrder;
        }
    }
}