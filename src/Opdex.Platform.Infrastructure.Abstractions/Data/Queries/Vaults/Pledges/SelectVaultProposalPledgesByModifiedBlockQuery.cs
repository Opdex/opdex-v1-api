using MediatR;
using Opdex.Platform.Domain.Models.Vaults;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.Pledges;

/// <summary>
/// Select vault proposal pledges by their modified block.
/// </summary>
public class SelectVaultProposalPledgesByModifiedBlockQuery : IRequest<IEnumerable<VaultProposalPledge>>
{
    /// <summary>
    /// Constructor to initialize a select vault proposal pledges by modified block query.
    /// </summary>
    /// <param name="blockHeight">The block height the records were modified at.</param>
    public SelectVaultProposalPledgesByModifiedBlockQuery(ulong blockHeight)
    {
        if (blockHeight < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
        }

        BlockHeight = blockHeight;
    }

    public ulong BlockHeight { get; }
}
