namespace PFM.Domain.Exceptions
{
    public class TransactionNotFoundException : BusinessRuleException
    {
        public TransactionNotFoundException(string id)
            : base("transaction-not-found", "Transaction not found.", $"Transaction with id '{id}' was not found.")
        {
        }
    }
}