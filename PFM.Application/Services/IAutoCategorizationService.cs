using PFM.Application.Dtos;

namespace PFM.Application.Services
{
    public interface IAutoCategorizationService
    {
        Task<AutoCategorizeResultDto> ApplyRulesAsync(CancellationToken cancellationToken);
    }
}