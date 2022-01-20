using MediatR;
using Opdex.Platform.Domain.Models.LiquidityPools;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;

/// <summary>
/// Retrieves all liquidity pools that have liquidity and have been who's summary modified block is older than the provided threshold.
/// </summary>
public class RetrieveLiquidityPoolsBySummaryModifiedBlockThresholdQuery : IRequest<IEnumerable<LiquidityPool>>
{
    /// <summary>
    /// Constructor to build a retrieve liquidity pools by summary modified block threshold query.
    /// </summary>
    /// <param name="blockThreshold">The number of blocks the latest summary update should be within.</param>
    public RetrieveLiquidityPoolsBySummaryModifiedBlockThresholdQuery(ulong blockThreshold)
    {
        if (blockThreshold == 0) throw new ArgumentOutOfRangeException(nameof(blockThreshold), "Block threshold must be greater than zero.");

        BlockThreshold = blockThreshold;
    }

    public ulong BlockThreshold { get; }
}
