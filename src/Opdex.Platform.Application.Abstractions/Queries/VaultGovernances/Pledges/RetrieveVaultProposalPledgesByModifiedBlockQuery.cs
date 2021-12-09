using MediatR;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Pledges;

/// <summary>
/// Retrieve all vault proposal pledges by their modified block.
/// </summary>
public class RetrieveVaultProposalPledgesByModifiedBlockQuery : IRequest<IEnumerable<VaultProposalPledge>>
{
    /// <summary>
    /// Constructor to initialize a retrieve vault proposal pledges by modified block query.
    /// </summary>
    /// <param name="blockHeight">The block height the records were modified at.</param>
    public RetrieveVaultProposalPledgesByModifiedBlockQuery(ulong blockHeight)
    {
        if (blockHeight < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
        }

        BlockHeight = blockHeight;
    }

    public ulong BlockHeight { get; }
}
