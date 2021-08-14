using MediatR;
using Opdex.Platform.Domain.Models.LiquidityPools.Snapshots;
using System;

namespace Opdex.Platform.Application.Abstractions.Commands.LiquidityPools
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
