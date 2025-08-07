namespace PFM.Domain.Exceptions
{
    public class TransactionNotFoundException : DomainException
    {
        public TransactionNotFoundException(string id)
            : base($"Transaction with id '{id}' was not found.") { }
    }
}