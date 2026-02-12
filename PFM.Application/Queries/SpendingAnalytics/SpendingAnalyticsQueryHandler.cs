using MediatR;
using PFM.Application.Dtos;
using PFM.Domain.Enums;
using PFM.Domain.Interfaces;

namespace PFM.Application.Queries.SpendingAnalytics
{
    public class SpendingAnalyticsQueryHandler : IRequestHandler<SpendingAnalyticsQuery, SpendingsByCategoryDto>
    {
        private readonly ITransactionRepository _txRepo;

        public SpendingAnalyticsQueryHandler(ITransactionRepository txRepo)
            => _txRepo = txRepo;

        public async Task<SpendingsByCategoryDto> Handle(SpendingAnalyticsQuery request, CancellationToken cancellationToken)
        {
            // default to current month if no range given
            var today = DateTime.UtcNow.Date;
            var firstOfMonth = new DateTime(today.Year, today.Month, 1);
            var lastOfMonth = firstOfMonth.AddMonths(1).AddDays(-1);

            var start = request.StartDate ?? firstOfMonth;
            var end = request.EndDate ?? lastOfMonth;

            // parse the CSV → List<TransactionDirection>
            TransactionDirectionEnum? directionEnum = null;
            if (!string.IsNullOrWhiteSpace(request.Direction)
                && Enum.TryParse<TransactionDirectionEnum>(
                     request.Direction.Trim(),
                     ignoreCase: true,
                     out var parsed))
            {
                directionEnum = parsed;
            }

            // fetch raw (CatCode, Amount, Count)
            var raw = await _txRepo.GetAnalyticsRawAsync(
                request.Category,
                start,
                end,
                directionEnum,
                cancellationToken);

            // map into spec DTO
            var groups = raw
                .Select(x => new SpendingInCategoryDto
                {
                    CatCode = x.CatCode,
                    Amount = x.Amount,
                    Count = x.Count
                })
                .ToList();

            return new SpendingsByCategoryDto { Groups = groups };
        }
    }
}
