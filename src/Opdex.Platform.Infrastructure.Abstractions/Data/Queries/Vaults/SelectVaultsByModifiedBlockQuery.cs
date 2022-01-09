using MediatR;
using Opdex.Platform.Domain.Models.Vaults;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults;

/// <summary>
/// Select vaults by their modified block.
/// </summary>
public class SelectVaultsByModifiedBlockQuery : IRequest<IEnumerable<Vault>>
{
    /// <summary>
    /// Constructor to initialize a select vaults by modified block query.
    /// </summary>
    /// <param name="blockHeight">The block height the records were modified at.</param>
    public SelectVaultsByModifiedBlockQuery(ulong blockHeight)
    {
        if (blockHeight < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
        }

        BlockHeight = blockHeight;
    }

    public ulong BlockHeight { get; }
}
