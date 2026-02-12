namespace PFM.Application.Dtos
{
    public class AutoCategorizeResultDto
    {
        public int CategorizedCount { get; set; }
        public int TotalTransactionCount { get; set; }
        public decimal CategorizedPercentage { get; set; }
    }
}
