using Microsoft.EntityFrameworkCore;
using PFM.Domain.Entities;
using PFM.Domain.Interfaces;

namespace PFM.Infrastructure.Persistence.Repositories
{
    public class CategoryRepository : ICategoryRepository, ICategoryReadRepository
    {
        private readonly AppDbContext _ctx;

        public CategoryRepository(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task UpsertRangeAsync(IEnumerable<Category> categories, CancellationToken cancellationToken)
        {
            foreach (var cat in categories)
            {
                var existing = await _ctx.Categories.FindAsync([cat.Code], cancellationToken);
                if (existing != null)
                {
                    existing.Name = cat.Name;
                    existing.ParentCode = cat.ParentCode;
                }
                else
                {
                    await _ctx.Categories.AddAsync(cat, cancellationToken);
                }
            }
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
            => _ctx.SaveChangesAsync(cancellationToken);

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