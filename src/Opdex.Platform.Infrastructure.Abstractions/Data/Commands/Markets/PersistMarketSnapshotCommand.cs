using System;
using MediatR;
using Opdex.Platform.Domain.Models.Markets;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Markets
{
    public class PersistMarketSnapshotCommand : IRequest<bool>
    {
        public PersistMarketSnapshotCommand(MarketSnapshot snapshot)
        {
            Snapshot = snapshot ?? throw new ArgumentNullException(nameof(snapshot));
        }
        
        public MarketSnapshot Snapshot { get; }
    }
}