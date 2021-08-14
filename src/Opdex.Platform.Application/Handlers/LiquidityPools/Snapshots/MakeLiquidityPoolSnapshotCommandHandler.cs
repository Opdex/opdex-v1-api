using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.LiquidityPools;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.LiquidityPools.Snapshots
{
    public class MakeLiquidityPoolSnapshotCommandHandler : IRequestHandler<MakeLiquidityPoolSnapshotCommand, bool>
    {
        private readonly IMediator _mediator;

        public MakeLiquidityPoolSnapshotCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<bool> Handle(MakeLiquidityPoolSnapshotCommand request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new PersistLiquidityPoolSnapshotCommand(request.Snapshot), cancellationToken);
        }
    }
}
