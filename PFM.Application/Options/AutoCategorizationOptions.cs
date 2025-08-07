namespace PFM.Application.Options
{
    public class AutoCategorizationOptions
    {
        public List<AutoCategorizationRule> Rules { get; set; } = new();
    }

    public class AutoCategorizationRule
    {
        public string Title { get; set; } = "";
        public string CatCode { get; set; } = "";
        public string Predicate { get; set; } = "";
    }
}
