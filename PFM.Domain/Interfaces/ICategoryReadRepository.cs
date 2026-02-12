using PFM.Domain.Entities;

namespace PFM.Domain.Interfaces
{
    public interface ICategoryReadRepository
    {
        Task<IReadOnlyList<Category>> ListAsync(string? parentCode, CancellationToken cancellationToken);
        Task<bool> ExistsAsync(IEnumerable<string> codes, CancellationToken cancellationToken);
        Task<Category?> GetByCodeAsync(string code, CancellationToken cancellationToken);
        Task<List<Category>> GetAllAsync(CancellationToken cancellationToken);
    }
}