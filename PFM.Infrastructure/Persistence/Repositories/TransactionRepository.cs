using Microsoft.EntityFrameworkCore;
using PFM.Domain.Entities;
using PFM.Domain.Exceptions;
using PFM.Domain.Interfaces;
using PFM.Domain.Enums;

namespace PFM.Infrastructure.Persistence.Repositories
{
    public class TransactionRepository : ITransactionRepository, ITransactionReadRepository
    {
        private readonly AppDbContext _ctx;

        public TransactionRepository(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task AddRangeAsync(IEnumerable<Transaction> transactions, CancellationToken cancellationToken)
            => await _ctx.Transactions.AddRangeAsync(transactions, cancellationToken);

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _ctx.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException dbEx)
            {
                var inner = dbEx.InnerException;
                // Detect Postgres unique‐constraint violation (23505), recast to domain exception.
                if (inner?.GetType().Name == "PostgresException"
                    && inner.GetType().GetProperty("SqlState")?.GetValue(inner) as string == "23505")
                {
                    throw new DuplicateTransactionException(
                        "One or more transaction IDs already exist.");
                }
                throw;
            }
        }

        public async Task<(IReadOnlyList<Transaction> Items, int TotalCount)> ListAsync(
            DateTime? startDate,
            DateTime? endDate,
            IEnumerable<TransactionKindEnum>? kinds,
            int page,
            int pageSize,
            string? sortBy,
            string? sortOrder,
            CancellationToken cancellationToken)
        {
            // Base query
            var query = _ctx.Transactions
                    .AsNoTracking()
                    .Include(t => t.Splits)
                    .AsQueryable();

            if (startDate.HasValue)
                query = query.Where(t => t.Date >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(t => t.Date <= endDate.Value);

            if (kinds != null && kinds.Any())
                query = query.Where(t => kinds.Contains(t.Kind));

            var sortMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
              { "id",                nameof(Transaction.Id) },
              { "date",              nameof(Transaction.Date) },
              { "amount",            nameof(Transaction.Amount) },
              { "beneficiary-name",  nameof(Transaction.BeneficiaryName) },
              { "description",       nameof(Transaction.Description) },
              { "currency",          nameof(Transaction.Currency) },
              { "mcc",               nameof(Transaction.Mcc) },
              { "kind",              nameof(Transaction.Kind) },
              { "cat-code",          nameof(Transaction.CatCode) }
            };

            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                var raw = sortBy.Split(',', StringSplitOptions.RemoveEmptyEntries)[0].Trim();
                if (sortMap.TryGetValue(raw, out var propName))
                {
                    var asc = sortOrder?.Equals("asc", StringComparison.OrdinalIgnoreCase) == true;
                    query = asc
                        ? query.OrderBy(e => EF.Property<object>(e, propName))
                        : query.OrderByDescending(e => EF.Property<object>(e, propName));
                }
            }
            else
            {
                query = query.OrderBy(t => t.Date);
            }


            // total before paging
            var total = await query.CountAsync(cancellationToken);

            // Apply paging
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, total);
        }

        public async Task<Transaction?> GetByIdAsync(string id, CancellationToken cancellationToken)
            => await _ctx.Transactions
                 .Include(t => t.Splits)
                 .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        public async Task<IReadOnlyList<(string CatCode, decimal Amount, int Count)
            >> GetAnalyticsRawAsync(
                string? rootCategory,
                DateTime? startDate,
                DateTime? endDate,
                TransactionDirectionEnum? direction,
                CancellationToken cancellationToken)
        {
            // 1) build base query with date & direction filters
            var tx = _ctx.Transactions.AsNoTracking().AsQueryable();
            if (startDate.HasValue) tx = tx.Where(t => t.Date >= startDate.Value);
            if (endDate.HasValue) tx = tx.Where(t => t.Date <= endDate.Value);
            if (direction.HasValue) tx = tx.Where(t => t.Direction == direction.Value);

            // 2) if they asked for a specific category ⇒ subcategory view
            if (!string.IsNullOrEmpty(rootCategory))
            {
                // find its immediate children
                var subCodes = await _ctx.Categories
                    .Where(c => c.ParentCode == rootCategory)
                    .Select(c => c.Code)
                    .ToListAsync(cancellationToken);

                // only those transactions
                tx = tx.Where(t => t.CatCode != null && subCodes.Contains(t.CatCode!));

                // group by subCode
                var subs = await tx
                    .GroupBy(t => t.CatCode!)
                    .Select(g => new { CatCode = g.Key, Amount = g.Sum(t => t.Amount), Count = g.Count() })
                    .ToListAsync(cancellationToken);

                return subs.Select(x => (x.CatCode, x.Amount, x.Count)).ToList();
            }

            // 3) otherwise ⇒ root view: join each transaction to its Category row,
            //    compute the “root code” (ParentCode if present, else Code),
            //    then group by that
            var rolledUp = await (
                from t in tx
                join c in _ctx.Categories on t.CatCode equals c.Code
                let rootCode = c.ParentCode ?? c.Code
                select new { rootCode, t.Amount }
            )
            .GroupBy(x => x.rootCode)
            .Select(g => new
            {
                CatCode = g.Key,
                Amount = g.Sum(x => x.Amount),
                Count = g.Count()
            })
            .ToListAsync(cancellationToken);

            return rolledUp
                .Select(x => (x.CatCode, x.Amount, x.Count))
                .ToList();
        }

    }
}
