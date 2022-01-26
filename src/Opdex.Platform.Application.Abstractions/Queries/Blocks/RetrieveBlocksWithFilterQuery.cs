using MediatR;
using Opdex.Platform.Domain.Models.Blocks;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Blocks;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Queries.Blocks;

public class RetrieveBlocksWithFilterQuery : IRequest<IEnumerable<Block>>
{
    public RetrieveBlocksWithFilterQuery(BlocksCursor cursor)
    {
        Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor));
    }

    public BlocksCursor Cursor { get; }
}
