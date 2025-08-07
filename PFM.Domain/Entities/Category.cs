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
    }
}