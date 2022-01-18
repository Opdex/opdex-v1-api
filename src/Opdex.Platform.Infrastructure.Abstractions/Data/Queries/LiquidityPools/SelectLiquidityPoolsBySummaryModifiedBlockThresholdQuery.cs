using MediatR;
using Opdex.Platform.Domain.Models.LiquidityPools;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.LiquidityPools;

public class SelectLiquidityPoolsBySummaryModifiedBlockThresholdQuery : IRequest<IEnumerable<LiquidityPool>>
{
    public SelectLiquidityPoolsBySummaryModifiedBlockThresholdQuery(ulong blockThreshold)
    {
        BlockThreshold = blockThreshold > 0 ? blockThreshold : throw new ArgumentOutOfRangeException(nameof(blockThreshold));
    }

    public ulong BlockThreshold { get; }
}
