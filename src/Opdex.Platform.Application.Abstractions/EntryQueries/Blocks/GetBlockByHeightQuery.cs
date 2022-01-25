using MediatR;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Models.Blocks;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Blocks;

public class GetBlockByHeightQuery : IRequest<BlockDto>
{
    public GetBlockByHeightQuery(ulong height)
    {
        Height = height > 0 ? height : throw new ArgumentOutOfRangeException(nameof(height), "Block height must be greater than 0.");
    }

    public ulong Height { get; }
}
