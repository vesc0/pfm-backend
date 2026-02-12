using PFM.Domain.Entities;

namespace PFM.Domain.Interfaces
{
    public interface ITransactionRepository
    {
        Task AddRangeAsync(IEnumerable<Transaction> transactions, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
