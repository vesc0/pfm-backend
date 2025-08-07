using PFM.Domain.Entities;

namespace PFM.Domain.Interfaces
{
    public interface ICategoryRepository
    {
        Task UpsertRangeAsync(IEnumerable<Category> categories, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
        Task<Category?> GetByCodeAsync(string code, CancellationToken cancellationToken);
        Task<List<Category>> GetAllAsync(CancellationToken cancellationToken);
        Task<bool> ExistsAsync(IEnumerable<string> codes, CancellationToken cancellationToken);
    }
}