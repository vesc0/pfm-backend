using FluentValidation;
using PFM.Domain.Enums;

namespace PFM.Application.Queries.Transaction
{
    public class ListTransactionsQueryValidator : AbstractValidator<ListTransactionsQuery>
    {
        private static readonly string[] AllowedSortBy = {
        "id", "date", "direction", "amount",
        "beneficiaryName", "description", "currency",
        "mcc", "kind", "catCode"
    };
        private static readonly string[] AllowedSortOrder = { "asc", "desc" };

        public ListTransactionsQueryValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThan(0)
                .WithErrorCode("invalid-page")
                .WithMessage("PageNumber must be greater than 0.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100)
                .WithErrorCode("invalid-page-size")
                .WithMessage("PageSize must be between 1 and 100.");

            When(x => x.StartDate.HasValue && x.EndDate.HasValue, () =>
            {
                RuleFor(x => x.EndDate)
                    .GreaterThanOrEqualTo(x => x.StartDate!.Value)
                    .WithErrorCode("invalid-date-range")
                    .WithMessage("End date must be the same or after start date.");
            });

            When(x => !string.IsNullOrWhiteSpace(x.Kinds), () =>
            {
                RuleFor(x => x.Kinds!)
                  .Must(csv =>
                  {
                      var parts = csv.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                   .Select(p => p.Trim());
                      return parts.All(p => Enum.IsDefined(typeof(TransactionKindEnum), p));
                  })
                  .WithErrorCode("invalid-kind")
                  .WithMessage("One or more provided kinds are invalid.");
            });

            When(x => !string.IsNullOrWhiteSpace(x.SortBy), () =>
            {
                RuleFor(x => x.SortBy!)
                    .Must(sb =>
                    {
                        var fields = sb.Split(',', StringSplitOptions.RemoveEmptyEntries);
                        return fields.All(f => AllowedSortBy.Contains(f));
                    })
                    .WithErrorCode("invalid-sort-by")
                    .WithMessage($"SortBy must be a comma-separated list of: {string.Join(", ", AllowedSortBy)}.");
            });

            When(x => !string.IsNullOrWhiteSpace(x.SortOrder), () =>
            {
                RuleFor(x => x.SortOrder!)
                    .Must(so =>
                    {
                        var orders = so
                            .Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(o => o.Trim().ToLowerInvariant());
                        return orders.All(o => AllowedSortOrder.Contains(o));
                    })
                    .WithErrorCode("invalid-sort-order")
                    .WithMessage("SortOrder must be 'asc' or 'desc'.");
            });
        }
    }
}