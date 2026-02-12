namespace PFM.Domain.Models
{
    public class AutoCategorizeResult
    {
        public int CategorizedCount { get; set; }
        public int TotalTransactionCount { get; set; }
        public decimal CategorizedPercentage { get; set; }
    }
}
