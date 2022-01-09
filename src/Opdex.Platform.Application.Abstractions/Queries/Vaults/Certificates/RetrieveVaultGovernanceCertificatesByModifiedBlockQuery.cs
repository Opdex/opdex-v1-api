using MediatR;
using Opdex.Platform.Domain.Models.Vaults;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Queries.Vaults.Certificates;

/// <summary>
/// Retrieve vault certificates based on their modified block.
/// </summary>
public class RetrieveVaultCertificatesByModifiedBlockQuery : IRequest<IEnumerable<VaultCertificate>>
{
    /// <summary>
    /// Constructor to initialize a retrieve vault certificates by modified block query.
    /// </summary>
    /// <param name="blockHeight">The block height to select modified certificates at.</param>
    public RetrieveVaultCertificatesByModifiedBlockQuery(ulong blockHeight)
    {
        if (blockHeight < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
        }

        BlockHeight = blockHeight;
    }

    public ulong BlockHeight { get; }
}
