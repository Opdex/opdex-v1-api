using MediatR;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.LiquidityPools;

/// <summary>
/// Processes stale liquidity pool and associated token snapshots when they are beyond a set block threshold.
/// </summary>
public class ProcessStaleLiquidityPoolSnapshotsCommand : IRequest<Unit>
{
    /// <summary>
    /// Constructor to build a process liquidity pool snapshots command.
    /// </summary>
    /// <param name="blockHeight">The current block height at which the snapshots are being refreshed.</param>
    /// <param name="blockTime">The current block median time at which the snapshots are being refreshed.</param>
    /// <param name="crsUsd">The price of CRS in USD at the block time.</param>
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
