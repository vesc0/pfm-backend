using PFM.Domain.Entities;

namespace PFM.Domain.Interfaces
{
    public interface ITransactionSplitRepository
    {
        Task<List<TransactionSplit>> GetByTransactionIdAsync(string transactionId, CancellationToken cancellationToken);
        Task DeleteByTransactionIdAsync(string transactionId, CancellationToken cancellationToken);
        Task AddRangeAsync(IEnumerable<TransactionSplit> splits, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}