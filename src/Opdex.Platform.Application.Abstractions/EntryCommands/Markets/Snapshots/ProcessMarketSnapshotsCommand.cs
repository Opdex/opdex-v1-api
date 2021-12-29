using MediatR;
using Opdex.Platform.Domain.Models.Markets;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Markets.Snapshots;

public class ProcessMarketSnapshotsCommand : IRequest<Unit>
{
    public ProcessMarketSnapshotsCommand(Market market, DateTime blockTime, ulong blockHeight)
    {
        Market = market ?? throw new ArgumentNullException(nameof(market), "Market must be provided.");
        BlockTime = !blockTime.Equals(default) ? blockTime : throw new ArgumentOutOfRangeException(nameof(blockTime), "Block time must be a valid date time.");
        BlockHeight = blockHeight > 0 ? blockHeight : throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
    }

    public Market Market { get; }
    public DateTime BlockTime { get; }
    public ulong BlockHeight { get; }
}
