using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PFM.Application.Commands.Transaction;
using PFM.Application.Options;
using PFM.Application.Services;
using PFM.Infrastructure.Persistence;

namespace PFM.Infrastructure.Services
{
    public class AutoCategorizationService : IAutoCategorizationService
    {
        private readonly AppDbContext _db;
        private readonly AutoCategorizationOptions _opts;

        public AutoCategorizationService(AppDbContext db, IOptions<AutoCategorizationOptions> opts)
        {
            _db = db;
            _opts = opts.Value;
        }

        public async Task<AutoCategorizeResultDto> ApplyRulesAsync(CancellationToken ct)
        {
            var updatedTransactionIds = new HashSet<string>();

            foreach (var rule in _opts.Rules)
            {
                var sql = $"SELECT * FROM transactions WHERE cat_code IS NULL AND {rule.Predicate}";
                var toUpdate = await _db.Transactions
                    .FromSqlRaw(sql)
                    .ToListAsync(ct);

                foreach (var tx in toUpdate)
                {
                    // Only update if not already updated and category is still null
                    if (!updatedTransactionIds.Contains(tx.Id) && string.IsNullOrEmpty(tx.CatCode))
                    {
                        tx.CatCode = rule.CatCode;
                        updatedTransactionIds.Add(tx.Id);
                    }
                }
            }

            if (updatedTransactionIds.Count > 0)
                await _db.SaveChangesAsync(ct);

            var totalTransactionCount = await _db.Transactions.CountAsync(ct);

            return new AutoCategorizeResultDto
            {
                CategorizedCount = updatedTransactionIds.Count,
                TotalTransactionCount = totalTransactionCount,
                CategorizedPercentage = totalTransactionCount > 0
                    ? Math.Round((decimal)updatedTransactionIds.Count / totalTransactionCount * 100, 2)
                    : 0
            };
        }
    }
}
