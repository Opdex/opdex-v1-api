using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses.Mining;
using System.Collections.Generic;

namespace Opdex.Platform.WebApi.Models.Requests.Wallets;

public sealed class MiningPositionFilterParameters : FilterParameters<MiningPositionsCursor>
{
    public MiningPositionFilterParameters()
    {
        // https://github.com/dotnet/aspnetcore/issues/37630 will not be bound if assigned as Enumerable.Empty<T>()
        MiningPools = new List<Address>();
        LiquidityPools = new List<Address>();
    }

    /// <summary>
    /// The specific mining pools to include.
    /// </summary>
    /// <example>[ "tNgQhNxvachxKGvRonk2S8nrpYi44carYv" ]</example>
    public IEnumerable<Address> MiningPools { get; set; }

    /// <summary>
    /// The specific liquidity pools to include.
    /// </summary>
    /// <example> [ "tMdZ2UfwJorAyErDvqNdVU8kmiLaykuE5L" ]</example>
    public IEnumerable<Address> LiquidityPools { get; set; }

    /// <summary>
    /// Includes zero amounts if true, otherwise filters out zero amounts if false. Default false.
    /// </summary>
    /// <example>true</example>
    public bool IncludeZeroAmounts { get; set; }

    /// <inheritdoc />
    protected override MiningPositionsCursor InternalBuildCursor()
    {
        if (EncodedCursor is null) return new MiningPositionsCursor(LiquidityPools, MiningPools, IncludeZeroAmounts, Direction, Limit, PagingDirection.Forward, default);
        Base64Extensions.TryBase64Decode(EncodedCursor, out var decodedCursor);
        MiningPositionsCursor.TryParse(decodedCursor, out var cursor);
        return cursor;
    }
}