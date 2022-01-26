using MediatR;
using Opdex.Platform.Domain.Models.Blocks;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Blocks;

public class SelectBlocksWithFilterQuery : IRequest<IEnumerable<Block>>
{
    public SelectBlocksWithFilterQuery(BlocksCursor cursor)
    {
        Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor));
    }

    public BlocksCursor Cursor { get; }
}
