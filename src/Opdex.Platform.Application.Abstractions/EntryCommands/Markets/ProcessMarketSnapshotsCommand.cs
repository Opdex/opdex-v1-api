using System;
using MediatR;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Markets
{
    public class ProcessMarketSnapshotsCommand : IRequest<Unit>
    {
        public ProcessMarketSnapshotsCommand(long marketId)
        {
            if (marketId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(marketId), $"{nameof(marketId)} must be greater than 0.");
            }

            MarketId = marketId;
        }

        public long MarketId { get; }
    }
}