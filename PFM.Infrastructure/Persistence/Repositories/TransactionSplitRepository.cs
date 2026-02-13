using Microsoft.EntityFrameworkCore;
using PFM.Domain.Entities;
using PFM.Domain.Interfaces;

namespace PFM.Infrastructure.Persistence.Repositories
{
    public class TransactionSplitRepository : ITransactionSplitRepository
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
    }
}