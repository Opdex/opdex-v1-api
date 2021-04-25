using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Markets;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Market
{
    public class PersistMarketSnapshotCommandHandler : IRequestHandler<PersistMarketSnapshotCommand, bool>
    {
        public Task<bool> Handle(PersistMarketSnapshotCommand request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}