using CsvHelper;
using CsvHelper.Configuration;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using PFM.Application.Dtos;
using PFM.Domain.Enums;
using PFM.Domain.Interfaces;
using System.Globalization;
using System.Text.RegularExpressions;
using DTransaction = PFM.Domain.Entities.Transaction;

namespace PFM.Application.Commands.Transaction
{
    public class ImportTransactionsCommandHandler
        : IRequestHandler<ImportTransactionsCommand, Unit>
    {
        private readonly ITransactionRepository _repository;
        private readonly IValidator<TransactionCsvDto> _dtoValidator;

        public ImportTransactionsCommandHandler(ITransactionRepository repository, IValidator<TransactionCsvDto> dtoValidator)
        {
            _repository = repository;
            _dtoValidator = dtoValidator;
        }

        public async Task<Unit> Handle(ImportTransactionsCommand request, CancellationToken cancellationToken)
        {
            using var reader = new StreamReader(request.CsvStream);
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                PrepareHeaderForMatch = args => args.Header.Trim().ToLowerInvariant(),
                MissingFieldFound = null,
                BadDataFound = null,
                TrimOptions = TrimOptions.Trim
            };

            List<TransactionCsvDto> records;
            try
            {
                using var csv = new CsvReader(reader, config);
                records = csv.GetRecords<TransactionCsvDto>().ToList();
            }
            catch (HeaderValidationException)
            {
                throw new FluentValidation.ValidationException(new[]
                { new ValidationFailure("file", "CSV is empty or the header row is missing/invalid.") });
            }
            catch (ReaderException)
            {
                throw new FluentValidation.ValidationException(new[]
                { new ValidationFailure("file", "CSV content is malformed and cannot be read.") });
            }
            catch (CsvHelperException)
            {
                throw new FluentValidation.ValidationException(new[]
                { new ValidationFailure("file", "CSV parsing failed due to invalid format.") });
            }

            // 1) DTO validation
            var failures = new List<FluentValidation.Results.ValidationFailure>();
            foreach (var dto in records)
            {
                var result = _dtoValidator.Validate(dto);
                if (!result.IsValid)
                    failures.AddRange(result.Errors);
            }
            if (failures.Any())
                throw new FluentValidation.ValidationException(failures);

            // 2) Map DTOs to domain entities
            var entities = records.Select(dto =>
            {
                string cleanedAmt = Regex.Replace(dto.Amount, @"[^\d\.\-]", "");

                return new DTransaction
                {
                    Id = dto.Id,
                    BeneficiaryName = dto.BeneficiaryName,
                    Date = DateTime.Parse(dto.Date, CultureInfo.InvariantCulture),
                    Direction = dto.Direction == "d"
                                        ? TransactionDirectionEnum.d
                                        : TransactionDirectionEnum.c,
                    Amount = decimal.Parse(
                                            cleanedAmt,
                                            NumberStyles.Number | NumberStyles.AllowDecimalPoint,
                                            CultureInfo.InvariantCulture),
                    Description = dto.Description,
                    Currency = dto.Currency.Trim(),
                    Mcc = string.IsNullOrWhiteSpace(dto.Mcc)
                                        ? null
                                        : (MccEnum?)int.Parse(dto.Mcc, CultureInfo.InvariantCulture),
                    Kind = Enum.Parse<TransactionKindEnum>(
                                            dto.Kind, ignoreCase: true)
                };
            }).ToList();

            // 3) Persist — all DB‐errors get translated inside the repository
            await _repository.AddRangeAsync(entities, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);


            return Unit.Value;
        }
    }
}
