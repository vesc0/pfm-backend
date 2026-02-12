using Microsoft.EntityFrameworkCore;
using PFM.Domain.Entities;
using PFM.Domain.Interfaces;

namespace PFM.Infrastructure.Persistence.Repositories
{
    public class TransactionSplitRepository : ITransactionSplitRepository, ITransactionSplitReadRepository
    {
        private readonly AppDbContext _ctx;

        public TransactionSplitRepository(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<List<TransactionSplit>> GetByTransactionIdAsync(string transactionId, CancellationToken cancellationToken)
        {
            return await _ctx.TransactionSplits
                .Where(s => s.TransactionId == transactionId)
                .ToListAsync(cancellationToken);
        }

        public async Task DeleteByTransactionIdAsync(string transactionId, CancellationToken cancellationToken)
        {
            var splits = await _ctx.TransactionSplits
                .Where(s => s.TransactionId == transactionId)
                .ToListAsync(cancellationToken);

            _ctx.TransactionSplits.RemoveRange(splits);
        }

        public async Task AddRangeAsync(IEnumerable<TransactionSplit> splits, CancellationToken cancellationToken)
        {
            await _ctx.TransactionSplits.AddRangeAsync(splits, cancellationToken);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
            => _ctx.SaveChangesAsync(cancellationToken);

        public async Task<Transaction?> GetByIdAsync(string id, CancellationToken cancellationToken)
            => await _ctx.Transactions.FindAsync([id], cancellationToken);

    }
}