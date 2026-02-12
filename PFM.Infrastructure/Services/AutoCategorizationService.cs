using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PFM.Domain.Entities;
using PFM.Domain.Enums;
using PFM.Domain.Exceptions;
using PFM.Domain.Interfaces;
using PFM.Domain.Models;
using PFM.Domain.Options;
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

        public async Task<AutoCategorizeResult> ApplyRulesAsync(CancellationToken ct)
        {
            var updatedTransactionIds = new HashSet<string>();

            foreach (var rule in _opts.Rules)
            {
                IQueryable<Transaction> query = _db.Transactions
                    .Where(t => t.CatCode == null);

                query = ApplyPredicate(query, rule.Predicate);

                var toUpdate = await query.ToListAsync(ct);

                foreach (var tx in toUpdate)
                {
                    if (updatedTransactionIds.Add(tx.Id))
                    {
                        tx.CatCode = rule.CatCode;
                    }
                }
            }

            if (updatedTransactionIds.Count > 0)
                await _db.SaveChangesAsync(ct);

            var totalTransactionCount = await _db.Transactions.CountAsync(ct);

            return new AutoCategorizeResult
            {
                CategorizedCount = updatedTransactionIds.Count,
                TotalTransactionCount = totalTransactionCount,
                CategorizedPercentage = totalTransactionCount > 0
                    ? Math.Round((decimal)updatedTransactionIds.Count / totalTransactionCount * 100, 2)
                    : 0
            };
        }

        private static IQueryable<Transaction> ApplyPredicate(
            IQueryable<Transaction> query,
            string predicate)
        {
            var parts = predicate.Split(" OR ", StringSplitOptions.RemoveEmptyEntries);

            IQueryable<Transaction>? combined = null;

            foreach (var part in parts)
            {
                var trimmed = part.Trim();

                // mcc = 1234
                var mccEq = Regex.Match(trimmed, @"^mcc\s*=\s*(\d+)$", RegexOptions.IgnoreCase);
                if (mccEq.Success)
                {
                    var value = int.Parse(mccEq.Groups[1].Value);
                    var mcc = (MccEnum)value;
                    combined = Or(combined, query.Where(t => t.Mcc == mcc));
                    continue;
                }

                // mcc IN (1,2,3)
                var mccIn = Regex.Match(trimmed, @"^mcc\s+IN\s*\(([\d,\s]+)\)$", RegexOptions.IgnoreCase);
                if (mccIn.Success)
                {
                    var values = mccIn.Groups[1].Value
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(v => (MccEnum)int.Parse(v.Trim()))
                        .ToList();

                    combined = Or(combined, query.Where(t => t.Mcc.HasValue && values.Contains(t.Mcc.Value)));
                    continue;
                }

                // LOWER(beneficiary_name) LIKE '%text%'
                var like = Regex.Match(
                    trimmed,
                    @"^LOWER\(beneficiary_name\)\s+LIKE\s+'%(.+)%'$",
                    RegexOptions.IgnoreCase);

                if (like.Success)
                {
                    var value = like.Groups[1].Value.ToLowerInvariant();
                    combined = Or(
                        combined,
                        query.Where(t =>
                            t.BeneficiaryName != null &&
                            EF.Functions.ILike(t.BeneficiaryName, $"%{value}%")));
                    continue;
                }

                throw new UnsupportedPredicateException(predicate);
            }

            return combined ?? query;
        }

        private static IQueryable<Transaction> Or(IQueryable<Transaction>? left, IQueryable<Transaction> right)
        {
            return left == null
                ? right
                : left.Concat(right).Distinct();
        }
    }
}
