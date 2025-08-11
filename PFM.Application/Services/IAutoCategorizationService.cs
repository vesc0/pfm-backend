using PFM.Application.Commands.Transaction;

namespace PFM.Application.Services
{
    public interface IAutoCategorizationService
    {
        Task<AutoCategorizeResultDto> ApplyRulesAsync(CancellationToken cancellationToken);
    }
}