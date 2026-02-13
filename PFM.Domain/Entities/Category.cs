using PFM.Domain.Exceptions;

namespace PFM.Domain.Entities
{
    public class Category
    {
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string? ParentCode { get; set; }
        public Category? Parent { get; set; }
        
        // Child categories in the hierarchy
        public ICollection<Category> Children { get; set; } = new List<Category>();

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Code))
            {
                throw new BusinessRuleException(
                    "category-code-empty",
                    "Category code must not be empty.");
            }

            if (string.IsNullOrWhiteSpace(Name))
            {
                throw new BusinessRuleException(
                    "category-name-empty",
                    "Category name must not be empty.");
            }

            if (ParentCode is not null && ParentCode == Code)
            {
                throw new BusinessRuleException(
                    "category-self-parent",
                    "A category cannot be its own parent.");
            }
        }
    }
}