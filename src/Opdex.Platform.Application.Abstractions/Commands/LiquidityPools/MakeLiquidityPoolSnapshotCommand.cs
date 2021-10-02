using MediatR;
using Opdex.Platform.Domain.Models.LiquidityPools.Snapshots;
using System;

namespace Opdex.Platform.Application.Abstractions.Commands.LiquidityPools
{
    public class MakeLiquidityPoolSnapshotCommand : IRequest<bool>
    {
        public MakeLiquidityPoolSnapshotCommand(LiquidityPoolSnapshot snapshot, ulong blockHeight)
        {
            if (blockHeight < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
            }

            Snapshot = snapshot ?? throw new ArgumentNullException(nameof(snapshot));
            BlockHeight = blockHeight;
        }

        public LiquidityPoolSnapshot Snapshot { get; }
        public ulong BlockHeight { get; }
    }
}
