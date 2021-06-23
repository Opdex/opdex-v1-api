using System;
using MediatR;
using Opdex.Platform.Domain.Models.Pools.Snapshots;

namespace Opdex.Platform.Application.Abstractions.Commands.Pools
{
    public class MakeLiquidityPoolSnapshotCommand : IRequest<bool>
    {
        public MakeLiquidityPoolSnapshotCommand(LiquidityPoolSnapshot snapshot)
        {
            Snapshot = snapshot ?? throw new ArgumentNullException(nameof(snapshot));
        }

        public LiquidityPoolSnapshot Snapshot { get; }
    }
}
