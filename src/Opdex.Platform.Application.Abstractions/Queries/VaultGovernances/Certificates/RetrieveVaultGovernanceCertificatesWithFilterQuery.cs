using MediatR;
using Opdex.Platform.Domain.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances.Certificates;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Certificates;

/// <summary>
/// Request to retrieve certificates in a vault.
/// </summary>
public class RetrieveVaultGovernanceCertificatesWithFilterQuery : IRequest<IEnumerable<VaultCertificate>>
{
    /// <summary>
    /// Creates a request to retrieve certificates in a vault.
    /// </summary>
    /// <param name="vaultId">Id of the vault.</param>
    /// <param name="cursor">Cursor filters.</param>
    public RetrieveVaultGovernanceCertificatesWithFilterQuery(ulong vaultId, VaultGovernanceCertificatesCursor cursor)
    {
        if (vaultId == 0) throw new ArgumentOutOfRangeException(nameof(vaultId), "Vault id must be greater than zero.");
        VaultId = vaultId;
        Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor), "Cursor must be set.");
    }

    public ulong VaultId { get; }
    public VaultGovernanceCertificatesCursor Cursor { get; }
}
