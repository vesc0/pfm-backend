using FluentValidation;

namespace PFM.Application.Commands.Transaction
{
    public class ImportTransactionsCommandValidator : AbstractValidator<ImportTransactionsCommand>
    {
        public ImportTransactionsCommandValidator()
        {
            RuleFor(x => x.CsvStream)
                .NotNull().WithMessage("A CSV file stream must be provided.")
                .Must(s => s.CanRead).WithMessage("The provided CSV stream is not readable.");
        }
    }
}
