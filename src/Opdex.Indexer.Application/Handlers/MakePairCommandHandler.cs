using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Indexer.Application.Abstractions.Commands;

namespace Opdex.Indexer.Application.Handlers
{
    public class MakePairCommandHandler : IRequestHandler<MakePairCommand, bool>
    {
        public Task<bool> Handle(MakePairCommand request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}