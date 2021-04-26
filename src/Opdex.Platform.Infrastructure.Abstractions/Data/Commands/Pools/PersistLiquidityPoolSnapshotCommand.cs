using System;
using MediatR;
using Opdex.Platform.Domain.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Pools
{
    public class PersistLiquidityPoolSnapshotCommand : IRequest<bool>
    {
        public PersistLiquidityPoolSnapshotCommand(LiquidityPoolSnapshot snapshot)
        {
            Snapshot = snapshot ?? throw new ArgumentNullException(nameof(snapshot));
        }
        
        public LiquidityPoolSnapshot Snapshot { get; }
    }
}