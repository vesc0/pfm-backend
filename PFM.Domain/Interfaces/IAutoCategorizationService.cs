using PFM.Domain.Models;
using PFM.Domain.Options;

namespace PFM.Domain.Interfaces
{
    public interface IAutoCategorizationService
    {
        Task<AutoCategorizeResult> ApplyRulesAsync(List<AutoCategorizationRule> rules, CancellationToken cancellationToken);
    }
}
