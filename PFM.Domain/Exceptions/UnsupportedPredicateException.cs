namespace PFM.Domain.Exceptions
{
    public class UnsupportedPredicateException : BusinessRuleException
    {
        public string Predicate { get; }

        public UnsupportedPredicateException(string predicate)
            : base(
                code: "unsupported-predicate",
                message: "Unsupported auto-categorization predicate",
                details: $"Unsupported auto-categorization predicate: '{predicate}'")
        {
            Predicate = predicate;
        }
    }
}