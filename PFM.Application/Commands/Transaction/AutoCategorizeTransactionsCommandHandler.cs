using MediatR;
using PFM.Application.Dtos;
using PFM.Domain.Interfaces;

namespace PFM.Application.Commands.Transaction
{
    public class AutoCategorizeTransactionsCommandHandler : IRequestHandler<AutoCategorizeTransactionsCommand, AutoCategorizeResultDto>
    {
        private readonly IAutoCategorizationService _svc;

        public AutoCategorizeTransactionsCommandHandler(IAutoCategorizationService svc)
            => _svc = svc;

        public async Task<AutoCategorizeResultDto> Handle(AutoCategorizeTransactionsCommand request, CancellationToken ct)
        {
            var result = await _svc.ApplyRulesAsync(ct);
            
            return new AutoCategorizeResultDto
            {
                CategorizedCount = result.CategorizedCount,
                TotalTransactionCount = result.TotalTransactionCount,
                CategorizedPercentage = result.CategorizedPercentage
            };
        }
    }
}
