using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.LiquidityPools;
using System.Collections.Generic;

namespace Opdex.Platform.WebApi.Models.Requests.LiquidityPools;

public class LiquidityPoolFilterParameters : FilterParameters<LiquidityPoolsCursor>
{
    public LiquidityPoolFilterParameters()
    {
        Markets = new List<Address>();
        LiquidityPools = new List<Address>();
        Tokens = new List<Address>();
    }

    /// <summary>
    /// Generic keyword search against liquidity pool addresses and names.
    /// </summary>
    /// <example>TBTC</example>
    public string Keyword { get; set; }

    /// <summary>
    /// Markets to search liquidity pools within.
    /// </summary>
    /// <example>[ "t7RorA7xQCMVYKPM1ibPE1NSswaLbpqLQb" ]</example>
    public IEnumerable<Address> Markets { get; set; }

    /// <summary>
    /// Liquidity pools to specifically filter for.
    /// </summary>
    /// <example>[ "tMdZ2UfwJorAyErDvqNdVU8kmiLaykuE5L", "tFCyMPURX9pVWXuXQqYY2hJmRcCrhmBPfY" ]</example>
    public IEnumerable<Address> LiquidityPools { get; set; }

    /// <summary>
    /// Tokens to specifically filter for.
    /// </summary>
    /// <example>[ "tGSk2dVENuqAQ2rNXbui37XHuurFCTqadD", "tF83sdXXt2nTkL7UyEYDVFMK4jTuYMbmR3" ]</example>
    public IEnumerable<Address> Tokens { get; set; }

    /// <summary>
    /// Staking status filter, default ignores filter.
    /// </summary>
    /// <example>Any</example>
    public LiquidityPoolStakingStatusFilter StakingStatus { get; set; }

    /// <summary>
    /// Nomination status filter, default ignores filter.
    /// </summary>
    /// <example>Enabled</example>
    public LiquidityPoolNominationStatusFilter NominationStatus { get; set; }

    /// <summary>
    /// Mining status filter, default ignores filter.
    /// </summary>
    /// <example>Enabled</example>
    public LiquidityPoolMiningStatusFilter MiningStatus { get; set; }

    /// <summary>
    /// The order to sort records by.
    /// </summary>
    /// <example>Volume</example>
    public LiquidityPoolOrderByType OrderBy { get; set; }

    /// <inheritdoc />
    protected override LiquidityPoolsCursor InternalBuildCursor()
    {
        if (EncodedCursor is null) return new LiquidityPoolsCursor(Keyword, Markets, LiquidityPools, Tokens, StakingStatus, NominationStatus, MiningStatus,
                                                                   OrderBy, Direction, Limit, PagingDirection.Forward, default);
        Base64Extensions.TryBase64Decode(EncodedCursor, out var decodedCursor);
        _ = LiquidityPoolsCursor.TryParse(decodedCursor, out var cursor);

        return cursor;
    }
}