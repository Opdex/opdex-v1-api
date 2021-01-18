using MediatR;

namespace Opdex.Indexer.Application.Abstractions.Commands
{
    public class MakeBlockCommand : IRequest<bool>
    {
        public MakeBlockCommand()
        {
            
        }
    }
}