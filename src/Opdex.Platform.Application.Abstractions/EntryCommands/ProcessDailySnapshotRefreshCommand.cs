using System;
using MediatR;

namespace Opdex.Platform.Application.Abstractions.EntryCommands;

/// <summary>
/// First block of the day, create all new daily snapshots for each token, pool, and market
/// Note: This is not necessarily built to scale... May need to be revisited in the future.
/// </summary>
public class ProcessDailySnapshotRefreshCommand : IRequest<Unit>
{
    /// <summary>
    /// Constructor creating new process daily snapshot refresh command.
    /// </summary>
    /// <param name="blockHeight">Current block height</param>
    /// <param name="blockTime">Current block time</param>
    /// <param name="crsUsd">Current CRS USd price</param>
    public ProcessDailySnapshotRefreshCommand(ulong blockHeight, DateTime blockTime, decimal crsUsd)
    {
        if (blockHeight == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(blockHeight));
        }

        if (blockTime.Equals(default))
        {
            throw new ArgumentOutOfRangeException(nameof(blockTime), $"{nameof(blockTime)} must be valid.");
        }

        if (crsUsd < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(crsUsd), $"{nameof(crsUsd)} must be greater than 0.");
        }

        BlockHeight = blockHeight;
        BlockTime = blockTime;
        CrsUsd = crsUsd;
    }

    public ulong BlockHeight { get; }
    public DateTime BlockTime { get; }
    public decimal CrsUsd { get; }
}