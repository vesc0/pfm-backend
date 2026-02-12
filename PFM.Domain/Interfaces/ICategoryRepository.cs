using PFM.Domain.Entities;

namespace PFM.Domain.Interfaces
{
    public interface ICategoryRepository
    {
        Task UpsertRangeAsync(IEnumerable<Category> categories, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}