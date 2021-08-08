using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Markets;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Markets.Snapshots
{
    public class MakeMarketSnapshotCommandHandler : IRequestHandler<MakeMarketSnapshotCommand, bool>
    {
        private readonly IMediator _mediator;

        public MakeMarketSnapshotCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<bool> Handle(MakeMarketSnapshotCommand request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new PersistMarketSnapshotCommand(request.Snapshot), cancellationToken);
        }
    }
}
