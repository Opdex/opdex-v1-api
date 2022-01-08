using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances.Certificates;

/// <summary>
/// Select vault governance certificates by the vault they're assigned in and the owner they're assigned to.
/// </summary>
public class SelectVaultGovernanceCertificatesByVaultIdAndOwnerQuery : IRequest<IEnumerable<VaultCertificate>>
{
    /// <summary>
    /// Constructor to create the select vault governance certificates by vault id and owner query.
    /// </summary>
    /// <param name="vaultId">The Id of the vault to lookup certificates by.</param>
    /// <param name="owner">The owner's address of the certificates to look up.</param>
    public SelectVaultGovernanceCertificatesByVaultIdAndOwnerQuery(ulong vaultId, Address owner)
    {
        VaultId = vaultId > 0 ? vaultId : throw new ArgumentOutOfRangeException(nameof(vaultId), "VaultId must be greater than zero.");
        Owner = owner != Address.Empty ? owner : throw new ArgumentNullException(nameof(owner), "Owner address must be provided.");
    }

    public ulong VaultId { get; }
    public Address Owner { get; }
}
