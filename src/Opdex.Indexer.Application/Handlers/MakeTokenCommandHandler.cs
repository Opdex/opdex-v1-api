using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Indexer.Application.Abstractions.Commands;

namespace Opdex.Indexer.Application.Handlers
{
    public class MakeTokenCommandHandler : IRequestHandler<MakeTokenCommand, bool>
    {
        public Task<bool> Handle(MakeTokenCommand request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}