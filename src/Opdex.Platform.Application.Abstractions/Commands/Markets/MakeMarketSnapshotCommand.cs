using MediatR;
using Opdex.Platform.Domain.Models.Markets;
using System;

namespace Opdex.Platform.Application.Abstractions.Commands.Markets
{
    public class MakeMarketSnapshotCommand : IRequest<bool>
    {
        public MakeMarketSnapshotCommand(MarketSnapshot snapshot)
        {
            Snapshot = snapshot ?? throw new ArgumentNullException(nameof(snapshot));
        }

        public MarketSnapshot Snapshot { get; }
    }
}
