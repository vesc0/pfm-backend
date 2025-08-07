using PFM.Domain.Entities;
using PFM.Domain.Enums;

namespace PFM.Domain.Interfaces
{
    public interface ITransactionReadRepository
    {
        Task<(IReadOnlyList<Transaction> Items, int TotalCount)> ListAsync(
            DateTime? startDate,
            DateTime? endDate,
            IEnumerable<TransactionKindEnum>? kinds,
            int page,
            int pageSize,
            string? sortBy,
            string? sortOrder,
            CancellationToken cancellationToken);

        Task<IReadOnlyList<(string CatCode, decimal Amount, int Count)>> GetAnalyticsRawAsync(
            string? rootCategory,
            DateTime? startDate,
            DateTime? endDate,
            TransactionDirectionEnum? direction,
            CancellationToken cancellationToken
        );

        Task<Transaction?> GetByIdAsync(string id, CancellationToken cancellationToken);
    }
}