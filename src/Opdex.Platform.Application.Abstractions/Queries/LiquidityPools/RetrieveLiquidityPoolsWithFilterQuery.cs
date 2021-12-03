using MediatR;
using Opdex.Platform.Domain.Models.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.LiquidityPools;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;

/// <summary>
/// Retrieve liquidity pools with filtering.
/// </summary>
public class RetrieveLiquidityPoolsWithFilterQuery : IRequest<IEnumerable<LiquidityPool>>
{
    /// <summary>
    /// Constructor to build a retrieve liquidity pools with filter query.
    /// </summary>
    /// <param name="cursor">The liquidity pools cursor to filter by.</param>
    public RetrieveLiquidityPoolsWithFilterQuery(LiquidityPoolsCursor cursor)
    {
        Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor), "Liquidity pools cursor must be set.");
    }

    public LiquidityPoolsCursor Cursor { get; }
}