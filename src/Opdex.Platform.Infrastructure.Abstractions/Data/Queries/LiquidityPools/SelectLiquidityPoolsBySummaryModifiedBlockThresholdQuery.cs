using MediatR;
using Opdex.Platform.Domain.Models.LiquidityPools;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.LiquidityPools;

/// <summary>
/// Selects all liquidity pools that have liquidity and have been who's summary modified block is older than the provided threshold.
/// </summary>
public class SelectLiquidityPoolsBySummaryModifiedBlockThresholdQuery : IRequest<IEnumerable<LiquidityPool>>
{
    /// <summary>
    /// Constructor to build a select liquidity pools by summary modified block threshold query.
    /// </summary>
    /// <param name="blockThreshold">The number of blocks the latest summary update should be within.</param>
    public SelectLiquidityPoolsBySummaryModifiedBlockThresholdQuery(ulong blockThreshold)
    {
        if (blockThreshold == 0) throw new ArgumentOutOfRangeException(nameof(blockThreshold), "Block threshold must be greater than zero.");

        BlockThreshold = blockThreshold;
    }

    public ulong BlockThreshold { get; }
}
