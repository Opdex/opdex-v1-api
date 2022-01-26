using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Blocks;

namespace Opdex.Platform.WebApi.Models.Requests.Blocks;

public sealed class BlockFilterParameters : FilterParameters<BlocksCursor>
{
    /// <inheritdoc />
    protected override BlocksCursor InternalBuildCursor()
    {
        if (EncodedCursor is null) return new BlocksCursor(Direction, Limit, PagingDirection.Forward, default);
        Base64Extensions.TryBase64Decode(EncodedCursor, out var decodedCursor);
        _ = BlocksCursor.TryParse(decodedCursor, out var cursor);
        return cursor;
    }
}
