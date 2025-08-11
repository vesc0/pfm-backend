namespace PFM.Domain.Exceptions
{
    public class BusinessRuleException : DomainException
    {
        public string Code { get; }
        public string? Details { get; }

        public BusinessRuleException(string code, string? message = null, string? details = null)
            : base(message ?? code)
        {
            Code = code;
            Details = details;
        }
    }

}
