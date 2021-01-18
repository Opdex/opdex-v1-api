using MediatR;

namespace Opdex.Indexer.Application.Abstractions.Commands
{
    public class MakeTransactionCommand : IRequest<bool>
    {
        public MakeTransactionCommand()
        {
            
        }
    }
}