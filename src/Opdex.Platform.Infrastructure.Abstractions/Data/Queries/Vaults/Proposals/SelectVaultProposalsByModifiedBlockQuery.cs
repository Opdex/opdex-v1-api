using MediatR;
using Opdex.Platform.Domain.Models.Vaults;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.Proposals;

/// <summary>
/// Select vault proposals by their modified block.
/// </summary>
public class SelectVaultProposalsByModifiedBlockQuery : IRequest<IEnumerable<VaultProposal>>
{
    /// <summary>
    /// Constructor to initialize a select vault proposals by modified block query.
    /// </summary>
    /// <param name="blockHeight">The block height the records were modified at.</param>
    public SelectVaultProposalsByModifiedBlockQuery(ulong blockHeight)
    {
        if (blockHeight < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
        }

        BlockHeight = blockHeight;
    }

    public ulong BlockHeight { get; }
}
