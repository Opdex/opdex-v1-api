using MediatR;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances.Certificates;

/// <summary>
/// Select vault certificates based on their modified block.
/// </summary>
public class SelectVaultGovernanceCertificatesByModifiedBlockQuery : IRequest<IEnumerable<VaultCertificate>>
{
    /// <summary>
    /// Constructor to initialize a select vault governance certificates by modified block query.
    /// </summary>
    /// <param name="blockHeight">The block height to select modified certificates at.</param>
    public SelectVaultGovernanceCertificatesByModifiedBlockQuery(ulong blockHeight)
    {
        if (blockHeight < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
        }

        BlockHeight = blockHeight;
    }

    public ulong BlockHeight { get; }
}
