using MediatR;
using Opdex.Platform.Domain.Models.Vaults;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.Certificates;

/// <summary>
/// Select vault certificates based on their modified block.
/// </summary>
public class SelectVaultCertificatesByModifiedBlockQuery : IRequest<IEnumerable<VaultCertificate>>
{
    /// <summary>
    /// Constructor to initialize a select vault certificates by modified block query.
    /// </summary>
    /// <param name="blockHeight">The block height to select modified certificates at.</param>
    public SelectVaultCertificatesByModifiedBlockQuery(ulong blockHeight)
    {
        if (blockHeight < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
        }

        BlockHeight = blockHeight;
    }

    public ulong BlockHeight { get; }
}
