using MediatR;
using Opdex.Platform.Domain.Models.Vaults;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Queries.Vaults;

/// <summary>
/// Retrieve vaults by their modified block.
/// </summary>
public class RetrieveVaultGovernancesByModifiedBlockQuery : IRequest<IEnumerable<VaultGovernance>>
{
    /// <summary>
    /// Constructor to initialize a retrieve vaults by modified block query.
    /// </summary>
    /// <param name="blockHeight">The block height the records were modified at.</param>
    public RetrieveVaultGovernancesByModifiedBlockQuery(ulong blockHeight)
    {
        if (blockHeight < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
        }

        BlockHeight = blockHeight;
    }

    public ulong BlockHeight { get; }
}
