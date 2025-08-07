using CsvHelper;
using CsvHelper.Configuration;
using FluentValidation;
using MediatR;
using PFM.Application.Dtos;
using PFM.Domain.Interfaces;
using System.Globalization;

namespace PFM.Application.Commands.Category
{
    public class ImportCategoriesCommandHandler : IRequestHandler<ImportCategoriesCommand, Unit>
    {
        private readonly ICategoryRepository _repo;
        private readonly IValidator<CategoryCsvDto> _dtoValidator;

        public ImportCategoriesCommandHandler(
            ICategoryRepository repo,
            IValidator<CategoryCsvDto> dtoValidator)
        {
            _repo = repo;
            _dtoValidator = dtoValidator;
        }

        public async Task<Unit> Handle(
            ImportCategoriesCommand request,
            CancellationToken cancellationToken)
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

            using var csv = new CsvReader(reader, config);
            var records = csv.GetRecords<CategoryCsvDto>().ToList();

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
            });

            // Upsert and save
            await _repo.UpsertRangeAsync(entities, cancellationToken);
            await _repo.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
