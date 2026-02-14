using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using PFM.Application.Dtos;
using PFM.Domain.Interfaces;
using PFM.Domain.Options;
using YamlDotNet.Serialization;

namespace PFM.Application.Commands.Transaction
{
    public class AutoCategorizeFromFileCommandHandler : IRequestHandler<AutoCategorizeFromFileCommand, AutoCategorizeResultDto>
    {
        private readonly IAutoCategorizationService _svc;
        private readonly IMapper _mapper;

        public AutoCategorizeFromFileCommandHandler(IAutoCategorizationService svc, IMapper mapper)
        {
            _svc = svc;
            _mapper = mapper;
        }

        public async Task<AutoCategorizeResultDto> Handle(AutoCategorizeFromFileCommand request, CancellationToken ct)
        {
            using var reader = new StreamReader(request.YamlStream);
            var yaml = await reader.ReadToEndAsync(ct);

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(YamlDotNet.Serialization.NamingConventions.PascalCaseNamingConvention.Instance)
                .Build();

            AutoCategorizationRoot root;
            try
            {
                root = deserializer.Deserialize<AutoCategorizationRoot>(yaml);
            }
            catch (Exception ex)
            {
                throw new ValidationException(new[]
                { new ValidationFailure("file", $"YAML is invalid or cannot be parsed: {ex.Message}") });
            }

            var opts = root?.AutoCategorization;

            if (opts?.Rules == null || opts.Rules.Count == 0)
                throw new ValidationException(new[]
                { new ValidationFailure("file", "The YAML file contains no rules.") });

            // Validate each rule
            var errors = new List<ValidationFailure>();
            for (var i = 0; i < opts.Rules.Count; i++)
            {
                var rule = opts.Rules[i];
                if (string.IsNullOrWhiteSpace(rule.CatCode))
                    errors.Add(new ValidationFailure($"Rules[{i}].CatCode", "Category code is required."));
                if (string.IsNullOrWhiteSpace(rule.Predicate))
                    errors.Add(new ValidationFailure($"Rules[{i}].Predicate", "Predicate is required."));
            }

            if (errors.Count > 0)
                throw new ValidationException(errors);

            var domainResult = await _svc.ApplyRulesAsync(opts.Rules, ct);
            return _mapper.Map<AutoCategorizeResultDto>(domainResult);
        }
    }

    internal class AutoCategorizationRoot
    {
        public AutoCategorizationOptions AutoCategorization { get; set; } = new();
    }
}
