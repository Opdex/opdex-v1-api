using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets;

namespace Opdex.Platform.WebApi.Models.Requests.Markets;

public class MarketFilterParameters : FilterParameters<MarketsCursor>
{
    public MarketType MarketType { get; set; }

    public MarketOrderByType OrderBy { get; set; }

    /// <inheritdoc />
    protected override MarketsCursor InternalBuildCursor()
    {
        if (EncodedCursor is null) return new MarketsCursor(MarketType, OrderBy, Direction, Limit, PagingDirection.Forward, default);
        Base64Extensions.TryBase64Decode(EncodedCursor, out var decodedCursor);
        MarketsCursor.TryParse(decodedCursor, out var cursor);
        return cursor;
    }
}
