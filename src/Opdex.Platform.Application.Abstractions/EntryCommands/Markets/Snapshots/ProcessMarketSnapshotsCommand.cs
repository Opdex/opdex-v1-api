using MediatR;
using Opdex.Platform.Domain.Models.Markets;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Markets.Snapshots
{
    public class ProcessMarketSnapshotsCommand : IRequest<Unit>
    {
        public ProcessMarketSnapshotsCommand(Market market, DateTime blockTime)
        {
            Market = market ?? throw new ArgumentNullException(nameof(market), $"Market must be provided.");
            BlockTime = blockTime;
        }

        public Market Market { get; }
        public DateTime BlockTime { get; }
    }
}
