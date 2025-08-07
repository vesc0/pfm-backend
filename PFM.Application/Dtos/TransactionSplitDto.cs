using System.Text.Json.Serialization;

namespace PFM.Application.Dtos
{
    public class TransactionSplitDto
    {
        public decimal Amount { get; set; }

        [JsonPropertyName("catcode")]
        public string CatCode { get; set; } = default!;
    }
}