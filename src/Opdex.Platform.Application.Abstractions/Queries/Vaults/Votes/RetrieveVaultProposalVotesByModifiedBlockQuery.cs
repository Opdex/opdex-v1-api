using MediatR;
using Opdex.Platform.Domain.Models.Vaults;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Queries.Vaults.Votes;

/// <summary>
/// Retrieve vault proposal votes by their modified block.
/// </summary>
public class RetrieveVaultProposalVotesByModifiedBlockQuery : IRequest<IEnumerable<VaultProposalVote>>
{
    /// <summary>
    /// Constructor to initialize a retrieve vault proposal votes by modified block query.
    /// </summary>
    /// <param name="blockHeight">The block height the records were modified at.</param>
    public RetrieveVaultProposalVotesByModifiedBlockQuery(ulong blockHeight)
    {
        if (blockHeight < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
        }

        BlockHeight = blockHeight;
    }

    public ulong BlockHeight { get; }
}
