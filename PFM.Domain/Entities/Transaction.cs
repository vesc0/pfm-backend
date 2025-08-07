using PFM.Domain.Enums;

namespace PFM.Domain.Entities
{
    public class Transaction
    {
        public string Id { get; set; } = default!;
        public string? BeneficiaryName { get; set; }
        public DateTime Date { get; set; }
        public TransactionDirectionEnum Direction { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public string Currency { get; set; } = default!;
        public MccEnum? Mcc { get; set; }
        public TransactionKindEnum Kind { get; set; }
        public string? CatCode { get; set; }

        // Navigation
        public virtual ICollection<TransactionSplit> Splits { get; set; } = new List<TransactionSplit>();
    }
}
