using MediatR;
using Opdex.Platform.Domain.Models.LiquidityPools;
using System;

namespace Opdex.Platform.Application.Abstractions.Commands.LiquidityPools
{
    public class MakeLiquidityPoolSummaryCommand : IRequest<long>
    {
        public MakeLiquidityPoolSummaryCommand(LiquidityPoolSummary summary)
        {
            Summary = summary ?? throw new ArgumentNullException(nameof(summary), "Liquidity pool summary must be set.");
        }

        public LiquidityPoolSummary Summary { get; }
    }
}
