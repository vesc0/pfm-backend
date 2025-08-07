using PFM.Domain.Entities;

namespace PFM.Domain.Interfaces
{
    public interface ICategoryReadRepository
    {
        Task<IReadOnlyList<Category>> ListAsync(string? parentCode, CancellationToken cancellationToken);
    }
}