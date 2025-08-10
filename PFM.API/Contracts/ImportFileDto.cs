using FluentValidation;

namespace PFM.API.Contracts;

public sealed class ImportFileDto
{
    public IFormFile? File { get; init; }
}

public sealed class ImportFileDtoValidator : AbstractValidator<ImportFileDto>
{
    public ImportFileDtoValidator()
    {
        RuleFor(x => x.File)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("A CSV file must be provided.")
            .Must(f => f!.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                .WithMessage("Only .csv files are supported.")
            .Must(f => f!.Length > 0)
                .WithMessage("The provided file is empty.");
    }
}
