using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Blocks;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Blocks;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Blocks;

public class GetBlocksWithFilterQuery : IRequest<BlocksDto>
{
    public GetBlocksWithFilterQuery(BlocksCursor cursor)
    {
        Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor));
    }

    public BlocksCursor Cursor { get; }
}
