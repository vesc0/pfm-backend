using MediatR;
using PFM.Application.Dtos;

namespace PFM.Application.Commands.Transaction
{
    public class AutoCategorizeTransactionsCommand : IRequest<AutoCategorizeResultDto> { }
}
