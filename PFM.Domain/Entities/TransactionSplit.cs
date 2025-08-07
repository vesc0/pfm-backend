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
    }
}