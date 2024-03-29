using MediatR;
using Opdex.Platform.Domain.Models.Markets;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Queries.Markets;

public class RetrieveMarketsByModifiedBlockQuery : IRequest<IEnumerable<Market>>
{
    public RetrieveMarketsByModifiedBlockQuery(ulong blockHeight)
    {
        if (blockHeight < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
        }

        BlockHeight = blockHeight;
    }

    public ulong BlockHeight { get; }
}