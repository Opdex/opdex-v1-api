using MediatR;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.LiquidityPools;

public class ProcessStaleLiquidityPoolSnapshotsCommand : IRequest<Unit>
{
    public ProcessStaleLiquidityPoolSnapshotsCommand(ulong blockHeight, DateTime blockTime, decimal crsUsd)
    {
        if (blockHeight == 0) throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
        if (blockTime.Equals(default)) throw new ArgumentOutOfRangeException(nameof(blockTime), "Block time must be a valid date time.");
        if (crsUsd <= 0) throw new ArgumentOutOfRangeException(nameof(crsUsd), "CRS USD must be greater than zero.");

        BlockHeight = blockHeight;
        BlockTime = blockTime;
        CrsUsd = crsUsd;
    }

    public ulong BlockHeight { get; }
    public DateTime BlockTime { get; }
    public decimal CrsUsd { get; }
}
