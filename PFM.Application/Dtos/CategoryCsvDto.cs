using CsvHelper.Configuration.Attributes;

namespace PFM.Application.Dtos
{
    public class CategoryCsvDto
    {
        [Name("code")]
        public string Code { get; set; } = default!;

        [Name("parent-code")]
        public string? ParentCode { get; set; }

        [Name("name")]
        public string Name { get; set; } = default!;
    }
}