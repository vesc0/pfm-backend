namespace PFM.Application.Services
{
    public interface IAutoCategorizationService
    {
        Task<int> ApplyRulesAsync(CancellationToken cancellationToken);
    }
}