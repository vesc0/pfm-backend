namespace PFM.Domain.Exceptions
{
    public class SplitAmountException : BusinessRuleException
    {
        public SplitAmountException()
            : base("split-amount-missmatch",
            "Split amount doesn't match transaction amount.",
            "Total amount of all splits must be equal to the transaction amount.")
        {
        }
    }
}