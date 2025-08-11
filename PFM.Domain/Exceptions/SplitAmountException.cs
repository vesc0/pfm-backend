namespace PFM.Domain.Exceptions
{
    public class SplitAmountException : BusinessRuleException
    {
        public SplitAmountException()
            : base("split-amount-over-transaction-amount",
            "Split amount is over transaction amount.",
            "Total amount of all splits must be equal to the transaction amount.")
        {
        }
    }
}