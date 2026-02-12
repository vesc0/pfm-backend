using PFM.Domain.Models;

namespace PFM.Domain.Interfaces
{
    public interface IAutoCategorizationService
    {
        Task<AutoCategorizeResult> ApplyRulesAsync(CancellationToken cancellationToken);
    }
}
