using MediatR;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Markets.Snapshots
{
    public class ProcessMarketSnapshotsCommand : IRequest<Unit>
    {
        public ProcessMarketSnapshotsCommand(long marketId, DateTime blockTime)
        {
            if (marketId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(marketId), $"{nameof(marketId)} must be greater than 0.");
            }

            MarketId = marketId;
            BlockTime = blockTime;
        }

        public long MarketId { get; }
        public DateTime BlockTime { get; }
    }
}
