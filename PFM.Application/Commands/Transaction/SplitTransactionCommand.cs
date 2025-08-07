using MediatR;
using PFM.Application.Dtos;

namespace PFM.Application.Commands.Transaction
{
    public class SplitTransactionCommand : IRequest<Unit>
    {
        public string TransactionId { get; set; } = default!;
        public List<TransactionSplitDto> Splits { get; set; } = new();
    }
}