using MediatR;

namespace PFM.Application.Commands.Transaction
{
    public class CategorizeTransactionCommand : IRequest<Unit>
    {
        public string TransactionId { get; }
        public string CatCode { get; }
        public CategorizeTransactionCommand(string transactionId, string catCode)
        {
            TransactionId = transactionId;
            CatCode = catCode;
        }
    }
}
