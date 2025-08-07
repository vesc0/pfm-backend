using FluentValidation;
using PFM.Domain.Enums;

namespace PFM.Application.Queries.SpendingAnalytics
{
    public class SpendingAnalyticsQueryValidator : AbstractValidator<SpendingAnalyticsQuery>
    {
        public SpendingAnalyticsQueryValidator()
        {
            RuleFor(q => q.StartDate)
                .LessThanOrEqualTo(q => q.EndDate)
                .When(q => q.StartDate.HasValue && q.EndDate.HasValue)
                .WithMessage("StartDate must be less than or equal to EndDate.");

            RuleFor(q => q.Direction)
                .Must(dir =>
                    string.IsNullOrWhiteSpace(dir) ||
                    Enum.TryParse<TransactionDirectionEnum>(dir, ignoreCase: true, out _)
                )
                .WithErrorCode("invalid-direction")
                .WithMessage("Direction is not valid.");

        }
    }
}