using System.Text.Json.Serialization;

namespace PFM.Application.Dtos
{
    public class SplitTransactionRequestDto
    {
        [JsonPropertyName("splits")]
        public List<TransactionSplitDto> Splits { get; set; } = new();
    }
}
