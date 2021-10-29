using MediatR;
using Opdex.Platform.Application.Abstractions.Models.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.LiquidityPools;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.LiquidityPools
{
    /// <summary>
    /// Get liquidity pools with filtering and pagination.
    /// </summary>
    public class GetLiquidityPoolsWithFilterQuery : IRequest<LiquidityPoolsDto>
    {
        /// <summary>
        /// Constructor to build a get liquidity pools with filter query.
        /// </summary>
        /// <param name="cursor">The liquidity pools cursor to filter by.</param>
        public GetLiquidityPoolsWithFilterQuery(LiquidityPoolsCursor cursor)
        {
            Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor), "Liquidity pools cursor must be set.");
        }

        public LiquidityPoolsCursor Cursor { get; }
    }
}
