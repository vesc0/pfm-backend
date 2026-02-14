using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Options;
using PFM.Application.Dtos;
using PFM.Domain.Interfaces;
using PFM.Domain.Options;

namespace PFM.Application.Commands.Transaction
{
    public class AutoCategorizeTransactionsCommandHandler : IRequestHandler<AutoCategorizeTransactionsCommand, AutoCategorizeResultDto>
    {
        private readonly IAutoCategorizationService _svc;
        private readonly IMapper _mapper;
        private readonly AutoCategorizationOptions _opts;

        public AutoCategorizeTransactionsCommandHandler(IAutoCategorizationService svc, IMapper mapper, IOptions<AutoCategorizationOptions> opts)
        {
            _svc = svc;
            _mapper = mapper;
            _opts = opts.Value;
        }

        public async Task<AutoCategorizeResultDto> Handle(AutoCategorizeTransactionsCommand request, CancellationToken ct)
        {
            if (_opts.Rules == null || _opts.Rules.Count == 0)
                throw new ValidationException(new[]
                { new ValidationFailure("rules", "No auto-categorization rules are configured.") });

            var result = await _svc.ApplyRulesAsync(_opts.Rules, ct);
            return _mapper.Map<AutoCategorizeResultDto>(result);
        }
    }
}
