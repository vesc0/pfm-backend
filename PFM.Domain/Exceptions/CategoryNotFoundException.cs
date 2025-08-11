namespace PFM.Domain.Exceptions
{
    public class CategoryNotFoundException : BusinessRuleException
    {
        public CategoryNotFoundException(string code)
            : base("category-not-found", "Category not found.", $"Category with code '{code}' was not found.")
        {
        }
    }
}