using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PFM.Application.Options;
using PFM.Application.Services;
using PFM.Infrastructure.Persistence;

namespace PFM.Infrastructure.Services
{
    public class AutoCategorizationService : IAutoCategorizationService
    {
        private readonly AppDbContext _db;
        private readonly AutoCategorizationOptions _opts;

        public AutoCategorizationService(
            AppDbContext db,
            IOptions<AutoCategorizationOptions> opts)
        {
            _db = db;
            _opts = opts.Value;
        }

        public async Task<int> ApplyRulesAsync(CancellationToken ct)
        {
            var totalUpdated = 0;

            foreach (var rule in _opts.Rules)
            {
                // Raw-SQL SELECT on unmapped transactions matching the predicate
                var sql = $"SELECT * FROM transactions WHERE cat_code IS NULL AND {rule.Predicate}";
                var toUpdate = await _db.Transactions
                    .FromSqlRaw(sql)
                    .ToListAsync(ct);

                foreach (var tx in toUpdate)
                {
                    tx.CatCode = rule.CatCode;
                    totalUpdated++;
                }
            }

            if (totalUpdated > 0)
                await _db.SaveChangesAsync(ct);

            return totalUpdated;
        }
    }
}
