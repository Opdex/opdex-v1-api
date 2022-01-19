using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses.Staking;
using System.Collections.Generic;

namespace Opdex.Platform.WebApi.Models.Requests.Wallets;

public sealed class StakingPositionFilterParameters : FilterParameters<StakingPositionsCursor>
{
    public StakingPositionFilterParameters()
    {
        LiquidityPools = new List<Address>();
    }

    /// <summary>
    /// The specific liquidity pools to include.
    /// </summary>
    /// <example>[ "t8WntmWKiLs1BdzoqPGXmPAYzUTpPb3DBw", "tMdZ2UfwJorAyErDvqNdVU8kmiLaykuE5L" ]</example>
    public IEnumerable<Address> LiquidityPools { get; set; }

    /// <summary>
    /// Includes zero amounts if true, otherwise filters out zero amounts if false. Default false.
    /// </summary>
    /// <example>true</example>
    public bool IncludeZeroAmounts { get; set; }

    /// <inheritdoc />
    protected override StakingPositionsCursor InternalBuildCursor()
    {
        if (EncodedCursor is null) return new StakingPositionsCursor(LiquidityPools, IncludeZeroAmounts, Direction, Limit, PagingDirection.Forward, default);
        Base64Extensions.TryBase64Decode(EncodedCursor, out var decodedCursor);
        StakingPositionsCursor.TryParse(decodedCursor, out var cursor);
        return cursor;
    }
}