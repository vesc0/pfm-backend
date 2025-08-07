namespace PFM.Application.Dtos
{
    public class SpendingAnalyticsResultDto
    {
        public string CategoryCode { get; set; } = default!;
        public string CategoryName { get; set; } = default!;
        public decimal Amount { get; set; }
        public List<SpendingAnalyticsResultDto> Children { get; set; } = new();
    }
}
