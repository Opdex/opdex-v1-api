using MediatR;
using Opdex.Platform.Domain.Models.LiquidityPools.Snapshots;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.LiquidityPools;

public class PersistLiquidityPoolSnapshotCommand : IRequest<bool>
{
    public PersistLiquidityPoolSnapshotCommand(LiquidityPoolSnapshot snapshot)
    {
        Snapshot = snapshot ?? throw new ArgumentNullException(nameof(snapshot));
    }

    public LiquidityPoolSnapshot Snapshot { get; }
}