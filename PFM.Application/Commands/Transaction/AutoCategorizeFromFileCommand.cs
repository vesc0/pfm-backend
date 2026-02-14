using MediatR;
using PFM.Application.Dtos;

namespace PFM.Application.Commands.Transaction
{
    public class AutoCategorizeFromFileCommand : IRequest<AutoCategorizeResultDto>
    {
        public Stream YamlStream { get; }

        public AutoCategorizeFromFileCommand(Stream yamlStream)
        {
            YamlStream = yamlStream ?? throw new ArgumentNullException(nameof(yamlStream));
        }
    }
}
