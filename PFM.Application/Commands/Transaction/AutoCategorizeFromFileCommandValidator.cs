using FluentValidation;

namespace PFM.Application.Commands.Transaction
{
    public class AutoCategorizeFromFileCommandValidator : AbstractValidator<AutoCategorizeFromFileCommand>
    {
        public AutoCategorizeFromFileCommandValidator()
        {
            RuleFor(x => x.YamlStream)
                .NotNull().WithMessage("A YAML file stream must be provided.")
                .Must(s => s.CanRead).WithMessage("The provided YAML stream is not readable.");
        }
    }
}
