using MediatR;

namespace PFM.Application.Commands.Category
{
    public class ImportCategoriesCommand : IRequest<Unit>
    {
        public Stream CsvStream { get; }

        public ImportCategoriesCommand(Stream csvStream)
        {
            CsvStream = csvStream ?? throw new ArgumentNullException(nameof(csvStream));
        }
    }
}
