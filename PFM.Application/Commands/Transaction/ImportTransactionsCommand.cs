using MediatR;

namespace PFM.Application.Commands.Transaction
{
    public class ImportTransactionsCommand : IRequest<Unit>
    {
        public Stream CsvStream { get; }

        public ImportTransactionsCommand(Stream csvStream)
        {
            CsvStream = csvStream ?? throw new ArgumentNullException(nameof(csvStream));
        }
    }
}
