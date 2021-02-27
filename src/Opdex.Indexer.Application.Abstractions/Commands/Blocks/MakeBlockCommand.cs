using MediatR;

namespace Opdex.Indexer.Application.Abstractions.Commands.Blocks
{
    public class MakeBlockCommand : IRequest<bool>
    {
        public MakeBlockCommand()
        {
            
        }
    }
}