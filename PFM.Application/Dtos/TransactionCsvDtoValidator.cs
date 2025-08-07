using FluentValidation;
using System.Globalization;
using System.Text.RegularExpressions;
using PFM.Domain.Enums;

namespace PFM.Application.Dtos
{
    public class TransactionCsvDtoValidator : AbstractValidator<TransactionCsvDto>
    {
        public TransactionCsvDtoValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required.");

            RuleFor(x => x.Date)
                .NotEmpty().WithMessage("Date is required.")
                .Must(d =>
                    DateTime.TryParse(
                        d,
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out _))
                .WithMessage("Date must be a valid date (e.g. 1/1/2021).");

            RuleFor(x => x.Direction)
                .NotEmpty().WithMessage("Direction is required.")
                .Must(d => d == "d" || d == "c")
                .WithMessage("Direction must be 'd' (debit) or 'c' (credit).");

            RuleFor(x => x.Amount)
                .NotEmpty().WithMessage("Amount is required.")
                .Must(a =>
                {
                    // strip any non-digit, non-dot, non-minus characters
                    var cleaned = Regex.Replace(a, @"[^\d\.\-]", "");
                    return decimal.TryParse(
                        cleaned,
                        NumberStyles.Number | NumberStyles.AllowDecimalPoint,
                        CultureInfo.InvariantCulture,
                        out _);
                })
                .WithMessage("Amount must be a valid number.");

            RuleFor(x => x.Currency)
                .NotEmpty().WithMessage("Currency is required.")
                .Length(3).WithMessage("Currency must be 3 letters.")
                .Matches("^[A-Z]{3}$").WithMessage("Currency must be uppercase ISO 4217 code.");

            RuleFor(x => x.Mcc)
                .Must(m => string.IsNullOrWhiteSpace(m) || (int.TryParse(m, out var v) && Enum.IsDefined(typeof(MccEnum), v)))
                .WithMessage("Mcc must be a valid MCC code.");

            RuleFor(x => x.Kind)
                .NotEmpty().WithMessage("Kind is required.")
                .Must(k => Enum.TryParse<TransactionKindEnum>(k, true, out _))
                .WithMessage("Kind must be one of the defined transaction kinds (dep, wdw, pmt, etc.).");
        }
    }
}
