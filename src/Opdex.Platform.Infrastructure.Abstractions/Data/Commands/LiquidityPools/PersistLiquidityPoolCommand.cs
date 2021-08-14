using MediatR;
using Opdex.Platform.Domain.Models.LiquidityPools;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.LiquidityPools
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
