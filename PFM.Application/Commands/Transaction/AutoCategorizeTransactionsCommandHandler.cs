using MediatR;
using PFM.Application.Services;

namespace PFM.Application.Commands.Transaction
{
    public class AutoCategorizeTransactionsCommandHandler
        : IRequestHandler<AutoCategorizeTransactionsCommand, AutoCategorizeResultDto>
    {
        private readonly IAutoCategorizationService _svc;

        public AutoCategorizeTransactionsCommandHandler(IAutoCategorizationService svc)
            => _svc = svc;

        public async Task<AutoCategorizeResultDto> Handle(AutoCategorizeTransactionsCommand request, CancellationToken ct)
        {
            var count = await _svc.ApplyRulesAsync(ct);
            return new AutoCategorizeResultDto { CategorizedCount = count };
        }
    }
}
