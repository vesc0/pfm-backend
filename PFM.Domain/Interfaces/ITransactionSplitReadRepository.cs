using PFM.Domain.Entities;

namespace PFM.Domain.Interfaces
{
    public interface ITransactionSplitReadRepository
    {
        Task<List<TransactionSplit>> GetByTransactionIdAsync(string transactionId, CancellationToken cancellationToken);
    }
}
