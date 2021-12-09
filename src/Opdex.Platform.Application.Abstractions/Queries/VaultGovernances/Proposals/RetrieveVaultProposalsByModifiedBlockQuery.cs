using MediatR;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Proposals;

/// <summary>
/// Retrieve vault proposals by their modified block.
/// </summary>
public class RetrieveVaultProposalsByModifiedBlockQuery : IRequest<IEnumerable<VaultProposal>>
{
    /// <summary>
    /// Constructor to initialize a retrieve vault proposals by modified block query.
    /// </summary>
    /// <param name="blockHeight">The block height the records were modified at.</param>
    public RetrieveVaultProposalsByModifiedBlockQuery(ulong blockHeight)
    {
        if (blockHeight < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
        }

        BlockHeight = blockHeight;
    }

    public ulong BlockHeight { get; }
}
