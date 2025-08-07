using CsvHelper.Configuration.Attributes;

namespace PFM.Application.Dtos
{
    public class TransactionCsvDto
    {
        [Name("id")]
        public string Id { get; set; } = default!;

        [Name("beneficiary-name")]
        public string? BeneficiaryName { get; set; }

        [Name("date")]
        public string Date { get; set; } = default!;

        [Name("direction")]
        public string Direction { get; set; } = default!;

        [Name("amount")]
        public string Amount { get; set; } = default!;

        [Name("description")]
        public string? Description { get; set; }

        [Name("currency")]
        public string Currency { get; set; } = default!;

        [Name("mcc")]
        public string? Mcc { get; set; }

        [Name("kind")]
        public string Kind { get; set; } = default!;
    }
}
