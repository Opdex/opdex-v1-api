using System;
using MediatR;
using Opdex.Platform.Domain.Models.Pools;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Pools
{
    public class PersistLiquidityPoolCommand : IRequest<long>
    {
        public PersistLiquidityPoolCommand(LiquidityPool liquidityPool)
        {
            Pool = liquidityPool ?? throw new ArgumentNullException(nameof(liquidityPool));
        }
        
        public LiquidityPool Pool { get;  }
    }
}