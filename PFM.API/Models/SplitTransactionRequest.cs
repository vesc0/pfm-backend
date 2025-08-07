namespace PFM.API.Models
{
    public class SplitTransactionRequest
    {
        public List<TransactionSplitRequestDto> Splits { get; set; } = new();
    }

    public class TransactionSplitRequestDto
    {
        public decimal Amount  { get; set; }
        public string  CatCode { get; set; } = default!;
    }
}
