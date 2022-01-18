using MediatR;
using Opdex.Platform.Domain.Models.LiquidityPools;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;

public class RetrieveLiquidityPoolsBySummaryModifiedBlockThresholdQuery : IRequest<IEnumerable<LiquidityPool>>
{
    public RetrieveLiquidityPoolsBySummaryModifiedBlockThresholdQuery(ulong blockThreshold)
    {
        if (BlockThreshold == 0) throw new ArgumentOutOfRangeException(nameof(blockThreshold), "BlockThreshold must be greater than zero.");

        BlockThreshold = blockThreshold;
    }

    public ulong BlockThreshold { get; }
}
