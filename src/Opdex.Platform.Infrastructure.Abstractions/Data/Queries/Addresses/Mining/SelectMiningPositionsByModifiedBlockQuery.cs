using MediatR;
using Opdex.Platform.Domain.Models.Addresses;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses.Mining;

public class SelectMiningPositionsByModifiedBlockQuery : IRequest<IEnumerable<AddressMining>>
{
    public SelectMiningPositionsByModifiedBlockQuery(ulong blockHeight)
    {
        if (blockHeight == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
        }

        BlockHeight = blockHeight;
    }

    public ulong BlockHeight { get; }
}