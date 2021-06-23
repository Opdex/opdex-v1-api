using System;
using MediatR;
using Opdex.Platform.Domain.Models.Pools;

namespace Opdex.Platform.Application.Abstractions.Commands.Pools
{
    public class MakeLiquidityPoolCommand : IRequest<long>
    {
        public MakeLiquidityPoolCommand(LiquidityPool liquidityPool)
        {
            LiquidityPool = liquidityPool ?? throw new ArgumentNullException(nameof(liquidityPool), $"{nameof(liquidityPool)} must be set.");
        }

        public LiquidityPool LiquidityPool { get; }
    }
}