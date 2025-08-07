using FluentValidation;

namespace PFM.Application.Dtos
{
    public class CategoryCsvDtoValidator : AbstractValidator<CategoryCsvDto>
    {
        public CategoryCsvDtoValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Category code is required.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Category name is required.");

            RuleFor(x => x.ParentCode)
              .Must(p => string.IsNullOrEmpty(p) || p.Trim().Length > 0)
              .WithMessage("ParentCode must be a non-empty string or omitted.");
        }
    }
}