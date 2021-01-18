using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Indexer.Application.Abstractions.Commands;

namespace Opdex.Indexer.Application.Handlers
{
    public class MakeTransactionCommandHandler : IRequestHandler<MakeTransactionCommand, bool>
    {
        public Task<bool> Handle(MakeTransactionCommand request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}