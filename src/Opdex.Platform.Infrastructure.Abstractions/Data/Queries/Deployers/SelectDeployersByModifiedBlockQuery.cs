using MediatR;
using Opdex.Platform.Domain.Models.Deployers;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Deployers;

public class SelectDeployersByModifiedBlockQuery : IRequest<IEnumerable<Deployer>>
{
    public SelectDeployersByModifiedBlockQuery(ulong blockHeight)
    {
        if (blockHeight < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
        }

        BlockHeight = blockHeight;
    }

    public ulong BlockHeight { get; }
}