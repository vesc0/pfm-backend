using System.Text.Json.Serialization;
using PFM.Domain.Enums;

namespace PFM.Application.Dtos
{
    // Data Transfer Object for returning transaction details.
    public class TransactionDto
    {
        public string Id { get; init; } = default!;
        public DateTime Date { get; init; }
        public TransactionDirectionEnum Direction { get; init; }
        public decimal Amount { get; init; }
        public string? BeneficiaryName { get; init; }
        public string? Description { get; init; }
        public string Currency { get; init; } = default!;
        public int? Mcc { get; init; }
        public TransactionKindEnum Kind { get; init; }
        
        [JsonPropertyName("catcode")]
        public string? CatCode { get; set; }
        public List<TransactionSplitDto> Splits { get; init; } = new();

    }
}