using MediatR;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances;

/// <summary>
/// Select vault governances by their modified block.
/// </summary>
public class SelectVaultGovernancesByModifiedBlockQuery : IRequest<IEnumerable<VaultGovernance>>
{
    /// <summary>
    /// Constructor to initialize a select vault governances by modified block query.
    /// </summary>
    /// <param name="blockHeight">The block height the records were modified at.</param>
    public SelectVaultGovernancesByModifiedBlockQuery(ulong blockHeight)
    {
        if (blockHeight < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
        }

        BlockHeight = blockHeight;
    }

    public ulong BlockHeight { get; }
}
