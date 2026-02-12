using Microsoft.EntityFrameworkCore;
using PFM.Domain.Entities;
using PFM.Domain.Exceptions;
using PFM.Domain.Interfaces;

namespace PFM.Infrastructure.Persistence.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _ctx;

        public CategoryRepository(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task UpsertRangeAsync(IEnumerable<Category> categories, CancellationToken cancellationToken)
        {
            await _ctx.Categories.AddRangeAsync(categories, cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _ctx.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException dbEx)
            {
                var inner = dbEx.InnerException;
                var isPgDup =
                    inner?.GetType().Name == "PostgresException" &&
                    (inner.GetType().GetProperty("SqlState")?.GetValue(inner) as string) == "23505";

                if (isPgDup)
                    throw new BusinessRuleException("category-already-exists",
                        message: "Category already exists.",
                        details: "One or more category codes already exist.");

                throw;
            }
        }

        public async Task<Category?> GetByCodeAsync(string code, CancellationToken cancellationToken)
            => await _ctx.Categories.FindAsync([code], cancellationToken);

        public async Task<List<Category>> GetAllAsync(CancellationToken cancellationToken)
            => await _ctx.Categories.ToListAsync(cancellationToken);

        public async Task<bool> ExistsAsync(IEnumerable<string> codes, CancellationToken cancellationToken)
        {
            var dbCodes = await _ctx.Categories
                .Where(c => codes.Contains(c.Code))
                .Select(c => c.Code)
                .ToListAsync(cancellationToken);

            return dbCodes.Count == codes.Distinct().Count();
        }

        public async Task<IReadOnlyList<Category>> ListAsync(string? parentCode, CancellationToken cancellationToken)
        {
            var query = _ctx.Categories.AsQueryable();
            if (!string.IsNullOrWhiteSpace(parentCode))
                query = query.Where(c => c.ParentCode == parentCode);

            return await query.ToListAsync(cancellationToken);
        }
    }
}