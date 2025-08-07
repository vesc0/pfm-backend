using FluentValidation;

namespace PFM.Application.Commands.Transaction
{
    public class CategorizeTransactionCommandValidator : AbstractValidator<CategorizeTransactionCommand>
    {
        public CategorizeTransactionCommandValidator()
        {
            RuleFor(x => x.TransactionId)
                .NotEmpty().WithMessage("TransactionId is required.");
            RuleFor(x => x.CatCode)
                .NotEmpty().WithMessage("Category code is required.");
        }
    }
}
