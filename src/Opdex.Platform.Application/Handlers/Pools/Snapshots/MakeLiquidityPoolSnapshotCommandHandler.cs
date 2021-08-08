using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Pools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Pools;

namespace Opdex.Platform.Application.Handlers.Pools.Snapshots
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