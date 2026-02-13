using PFM.Domain.Exceptions;

namespace PFM.Domain.Entities
{
    public class TransactionSplit
    {
        public int Id { get; set; } // PK
        public string TransactionId { get; set; } = default!; // FK to Transaction
        public decimal Amount { get; set; }
        public string CatCode { get; set; } = default!; // FK to Category
        public Category Category { get; set; } = default!;

        // Navigation
        public virtual Transaction? Transaction { get; set; }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(TransactionId))
            {
                throw new BusinessRuleException(
                    "split-transaction-id-empty",
                    "Transaction ID is required for a split.");
            }

            if (Amount <= 0)
            {
                throw new BusinessRuleException(
                    "split-amount-not-positive",
                    "Split amount must be greater than zero.");
            }

            if (string.IsNullOrWhiteSpace(CatCode))
            {
                throw new BusinessRuleException(
                    "split-catcode-empty",
                    "Category code is required for a split.");
            }
        }
    }
}