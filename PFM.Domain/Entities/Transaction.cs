using PFM.Domain.Enums;
using PFM.Domain.Exceptions;

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

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Id))
            {
                throw new BusinessRuleException(
                    "transaction-id-empty",
                    "Transaction ID must not be empty.");
            }

            if (Amount <= 0)
            {
                throw new BusinessRuleException(
                    "transaction-amount-not-positive",
                    "Transaction amount must be greater than zero.");
            }

            if (string.IsNullOrWhiteSpace(Currency))
            {
                throw new BusinessRuleException(
                    "transaction-currency-empty",
                    "Transaction currency must not be empty.");
            }

            if (Splits.Count > 0)
            {
                ValidateSplits();
            }
        }

        private void ValidateSplits()
        {
            if (Splits.Count < 2)
            {
                throw new BusinessRuleException(
                    "transaction-splits-minimum",
                    "A split transaction must have at least 2 splits.");
            }

            foreach (var split in Splits)
            {
                split.Validate();
            }

            var duplicateCatCodes = Splits
                .GroupBy(s => s.CatCode)
                .Any(g => g.Count() > 1);

            if (duplicateCatCodes)
            {
                throw new BusinessRuleException(
                    "transaction-splits-duplicate-catcode",
                    "Splits must not contain duplicate category codes.");
            }

            var splitsSum = Splits.Sum(s => s.Amount);
            if (splitsSum != Amount)
            {
                throw new SplitAmountException();
            }
        }
    }
}
