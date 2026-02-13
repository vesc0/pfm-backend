using CsvHelper;
using CsvHelper.Configuration;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using PFM.Application.Dtos;
using PFM.Domain.Interfaces;
using System.Globalization;

namespace PFM.Application.Commands.Category
{
    public class ImportCategoriesCommandHandler : IRequestHandler<ImportCategoriesCommand, Unit>
    {
        private readonly IUnitOfWork _uow;
        private readonly IValidator<CategoryCsvDto> _dtoValidator;

        public ImportCategoriesCommandHandler(IUnitOfWork uow, IValidator<CategoryCsvDto> dtoValidator)
        {
            _uow = uow;
            _dtoValidator = dtoValidator;
        }

        public async Task<Unit> Handle(ImportCategoriesCommand request, CancellationToken cancellationToken)
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

            List<CategoryCsvDto> records;
            try
            {
                using var csv = new CsvReader(reader, config);
                records = csv.GetRecords<CategoryCsvDto>().ToList();
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

            // Validate each DTO
            var failures = records
                .SelectMany(dto => _dtoValidator.Validate(dto).Errors)
                .ToList();
            if (failures.Any())
                throw new FluentValidation.ValidationException(failures);

            // Map to domain entities
            var entities = records.Select(dto => new PFM.Domain.Entities.Category
            {
                Code = dto.Code.Trim(),
                Name = dto.Name.Trim(),
                ParentCode = string.IsNullOrWhiteSpace(dto.ParentCode)
                               ? null
                               : dto.ParentCode.Trim()
            }).ToList();

            // Domain validation
            foreach (var entity in entities)
                entity.Validate();

            // Upsert and save
            await _uow.Categories.UpsertRangeAsync(entities, cancellationToken);
            await _uow.CompleteAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
