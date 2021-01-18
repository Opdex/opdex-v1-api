using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Indexer.Application.Abstractions.Commands;

namespace Opdex.Indexer.Application.Handlers
{
    public class MakeBlockCommandHandler : IRequestHandler<MakeBlockCommand, bool>
    {
        public Task<bool> Handle(MakeBlockCommand request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}