namespace PFM.Domain.Exceptions
{
    public class DuplicateTransactionException : DomainException
    {
        public DuplicateTransactionException(string id)
            : base($"A transaction with id '{id}' already exists.") { }
    }
}