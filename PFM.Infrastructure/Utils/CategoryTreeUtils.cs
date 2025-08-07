using PFM.Domain.Entities;

namespace PFM.Infrastructure.Utils
{
    public static class CategoryTreeUtils
    {
        public static List<string> GetDescendantCodes(string rootCode, List<Category> allCategories)
        {
            var codes = new List<string>();
            void AddDescendants(string parent)
            {
                codes.Add(parent);
                var children = allCategories
                    .Where(c => c.ParentCode == parent)
                    .Select(c => c.Code)
                    .ToList();
                foreach (var child in children)
                    AddDescendants(child);
            }
            AddDescendants(rootCode);
            return codes;
        }
    }
}
