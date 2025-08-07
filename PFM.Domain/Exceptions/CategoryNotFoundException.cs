namespace PFM.Domain.Exceptions
{
    public class CategoryNotFoundException : DomainException
    {
        public CategoryNotFoundException(string code)
            : base($"Category with code '{code}' was not found.") { }
    }
}