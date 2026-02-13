using AutoMapper;
using MediatR;
using PFM.Application.Dtos;
using PFM.Domain.Interfaces;

namespace PFM.Application.Commands.Transaction
{
    public class AutoCategorizeTransactionsCommandHandler : IRequestHandler<AutoCategorizeTransactionsCommand, AutoCategorizeResultDto>
    {
        private readonly IAutoCategorizationService _svc;
        private readonly IMapper _mapper;

        public AutoCategorizeTransactionsCommandHandler(IAutoCategorizationService svc, IMapper mapper)
        {
            _svc = svc;
            _mapper = mapper;
        }

        public async Task<AutoCategorizeResultDto> Handle(AutoCategorizeTransactionsCommand request, CancellationToken ct)
        {
            var result = await _svc.ApplyRulesAsync(ct);
            return _mapper.Map<AutoCategorizeResultDto>(result);
        }
    }
}
