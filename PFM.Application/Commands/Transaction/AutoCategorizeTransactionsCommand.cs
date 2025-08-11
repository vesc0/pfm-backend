using MediatR;

namespace PFM.Application.Commands.Transaction
{
    public class AutoCategorizeTransactionsCommand : IRequest<AutoCategorizeResultDto> { }

    public class AutoCategorizeResultDto
    {
        public int CategorizedCount { get; set; }
        public int TotalTransactionCount { get; set; }
        public decimal CategorizedPercentage { get; set; }
    }
}
