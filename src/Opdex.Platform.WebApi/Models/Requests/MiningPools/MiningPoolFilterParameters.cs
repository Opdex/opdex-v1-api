using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.MiningPools;
using System.Collections.Generic;

namespace Opdex.Platform.WebApi.Models.Requests.MiningPools;

public sealed class MiningPoolFilterParameters : FilterParameters<MiningPoolsCursor>
{
    public MiningPoolFilterParameters()
    {
        LiquidityPools = new List<Address>();
    }

    /// <summary>
    /// The liquidity pools used for mining.
    /// </summary>
    /// <example>[ "tMdZ2UfwJorAyErDvqNdVU8kmiLaykuE5L", "t8WntmWKiLs1BdzoqPGXmPAYzUTpPb3DBw" ]</example>
    public IEnumerable<Address> LiquidityPools { get; set; }

    /// <summary>
    /// Mining pool activity status.
    /// </summary>
    /// <example>Active</example>
    public MiningStatusFilter MiningStatus { get; set; }

    /// <inheritdoc />
    protected override MiningPoolsCursor InternalBuildCursor()
    {
        if (EncodedCursor is null) return new MiningPoolsCursor(LiquidityPools, MiningStatus, Direction, Limit, PagingDirection.Forward, default);
        Base64Extensions.TryBase64Decode(EncodedCursor, out var decodedCursor);
        MiningPoolsCursor.TryParse(decodedCursor, out var cursor);
        return cursor;
    }
}