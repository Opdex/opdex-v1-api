using System;
using MediatR;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Domain.Models.Blocks;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Blocks;

public class SelectBlockByHeightQuery : FindQuery<Block>
{
    public SelectBlockByHeightQuery(ulong height, bool findOrThrow = true) : base(findOrThrow)
    {
        if (height < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(height));
        }

        Height = height;
    }
        
    public ulong Height { get; }
}