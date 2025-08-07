using FluentValidation;

namespace PFM.Application.Commands.Category
{
    public class ImportCategoriesCommandValidator : AbstractValidator<ImportCategoriesCommand>
    {
        public ImportCategoriesCommandValidator()
        {
            RuleFor(x => x.CsvStream)
                .NotNull().WithMessage("A CSV file stream must be provided.")
                .Must(s => s.CanRead).WithMessage("The provided CSV stream is not readable.");
        }
    }
}