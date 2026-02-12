using PFM.Application.Dtos;

namespace PFM.API.Contracts
{
    public class SplitTransactionRequest
    {
        public List<TransactionSplitDto> Splits { get; set; } = new();
    }
}
