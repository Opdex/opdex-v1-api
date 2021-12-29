using MediatR;
using Opdex.Platform.Domain.Models.Markets;
using System;

namespace Opdex.Platform.Application.Abstractions.Commands.Markets;

public class MakeMarketSnapshotCommand : IRequest<bool>
{
    public MakeMarketSnapshotCommand(MarketSnapshot snapshot, ulong blockHeight)
    {
        Snapshot = snapshot ?? throw new ArgumentNullException(nameof(snapshot));
        BlockHeight = blockHeight > 0 ? blockHeight : throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
    }

    public MarketSnapshot Snapshot { get; }
    public ulong BlockHeight { get; }
}
