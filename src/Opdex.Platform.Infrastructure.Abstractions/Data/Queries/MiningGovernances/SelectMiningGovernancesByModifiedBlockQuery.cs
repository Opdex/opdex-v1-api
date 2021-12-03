using MediatR;
using Opdex.Platform.Domain.Models.MiningGovernances;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.MiningGovernances;

public class SelectMiningGovernancesByModifiedBlockQuery : IRequest<IEnumerable<MiningGovernance>>
{
    public SelectMiningGovernancesByModifiedBlockQuery(ulong blockHeight)
    {
        if (blockHeight < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
        }

        BlockHeight = blockHeight;
    }

    public ulong BlockHeight { get; }
}