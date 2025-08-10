namespace PFM.Domain.Exceptions
{
    public sealed class BusinessRuleException : DomainException
    {
        public string Code { get; }
        public string? Tag { get; }

        public BusinessRuleException(string code, string? tag = null, string? message = null)
            : base(message ?? code)
        {
            Code = code;
            Tag = tag;
        }
    }
}
