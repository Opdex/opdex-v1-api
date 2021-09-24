using MediatR;
using Opdex.Platform.Domain.Models.LiquidityPools;
using System;

namespace Opdex.Platform.Application.Abstractions.Commands.LiquidityPools
{
    public class MakeLiquidityPoolCommand : IRequest<long>
    {
        public MakeLiquidityPoolCommand(LiquidityPool liquidityPool)
        {
            LiquidityPool = liquidityPool ?? throw new ArgumentNullException(nameof(liquidityPool), "Liquidity pool must be set.");
        }

        public LiquidityPool LiquidityPool { get; }
    }
}
