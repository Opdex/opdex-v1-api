using MediatR;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Markets.Snapshots
{
    public class ProcessMarketSnapshotsCommand : IRequest<Unit>
    {
        public ProcessMarketSnapshotsCommand(ulong marketId, DateTime blockTime)
        {
            if (marketId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(marketId), $"{nameof(marketId)} must be greater than 0.");
            }

            MarketId = marketId;
            BlockTime = blockTime;
        }

        public ulong MarketId { get; }
        public DateTime BlockTime { get; }
    }
}
