using System.Text.Json.Serialization;

namespace PFM.Application.Dtos
{
    public class SpendingsByCategoryDto
    {
        public List<SpendingInCategoryDto> Groups { get; set; } = new();
    }

    public class SpendingInCategoryDto
    {
        [JsonPropertyName("catcode")]
        public string CatCode { get; set; } = default!;
        public decimal Amount { get; set; }
        public int Count { get; set; }
    }
}