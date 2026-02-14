using FluentValidation;

namespace PFM.API.Contracts;

public sealed class ImportYamlFileRequest
{
    public IFormFile? File { get; init; }
}

public sealed class ImportYamlFileRequestValidator : AbstractValidator<ImportYamlFileRequest>
{
    public ImportYamlFileRequestValidator()
    {
        RuleFor(x => x.File)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("A YAML file must be provided.")
            .Must(f => f!.FileName.EndsWith(".yml", StringComparison.OrdinalIgnoreCase)
                     || f.FileName.EndsWith(".yaml", StringComparison.OrdinalIgnoreCase))
                .WithMessage("Only .yml/.yaml files are supported.")
            .Must(f => f!.Length > 0)
                .WithMessage("The provided file is empty.");
    }
}
