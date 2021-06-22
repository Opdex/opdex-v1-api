using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.EntryCommands.Markets;

namespace Opdex.Platform.Application.EntryHandlers.Markets
{
    public class ProcessMarketSnapshotsCommandHandler : IRequestHandler<ProcessMarketSnapshotsCommand, Unit>
    {
        private readonly IMediator _mediator;

        public ProcessMarketSnapshotsCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<Unit> Handle(ProcessMarketSnapshotsCommand request, CancellationToken cancellationToken)
        {
            // Get all daily market snapshots
            // Get all daily liquidity pool snapshots in market
            // Process market snapshot from lp snapshots

            return Unit.Value;
        }
    }
}