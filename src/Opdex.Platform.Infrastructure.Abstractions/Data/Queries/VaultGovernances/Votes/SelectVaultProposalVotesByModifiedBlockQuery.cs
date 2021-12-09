using MediatR;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances.Votes;

/// <summary>
/// Select vault proposal votes by their modified block.
/// </summary>
public class SelectVaultProposalVotesByModifiedBlockQuery : IRequest<IEnumerable<VaultProposalVote>>
{
    /// <summary>
    /// Constructor to initialize a select vault proposal votes by modified block query.
    /// </summary>
    /// <param name="blockHeight">The block height the records were modified at.</param>
    public SelectVaultProposalVotesByModifiedBlockQuery(ulong blockHeight)
    {
        if (blockHeight < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
        }

        BlockHeight = blockHeight;
    }

    public ulong BlockHeight { get; }
}
