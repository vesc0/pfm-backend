using FluentValidation;

namespace PFM.Application.Commands.Transaction
{
    public class SplitTransactionCommandValidator : AbstractValidator<SplitTransactionCommand>
    {
        public SplitTransactionCommandValidator()
        {
            RuleFor(x => x.TransactionId)
                .NotEmpty().WithMessage("TransactionId is required.");

            RuleFor(x => x.Splits)
                .NotNull().WithMessage("Splits are required.")
                .Must(s => s.Count >= 2).WithMessage("At least 2 splits are required.");

            RuleForEach(x => x.Splits).SetValidator(new TransactionSplitDtoValidator());
        }
    }

    public class TransactionSplitDtoValidator : AbstractValidator<Dtos.TransactionSplitDto>
    {
        public TransactionSplitDtoValidator()
        {
            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Split amount must be positive.");

            RuleFor(x => x.CatCode)
                .NotEmpty().WithMessage("CatCode is required.");
        }
    }
}