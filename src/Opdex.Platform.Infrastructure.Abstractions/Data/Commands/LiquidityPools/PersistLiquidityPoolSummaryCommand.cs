using MediatR;
using Opdex.Platform.Domain.Models.LiquidityPools;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.LiquidityPools
{
    public class PersistLiquidityPoolSummaryCommand : IRequest<long>
    {
        public PersistLiquidityPoolSummaryCommand(LiquidityPoolSummary summary)
        {
            Summary = summary ?? throw new ArgumentNullException(nameof(summary));
        }

        public LiquidityPoolSummary Summary { get; }
    }
}
