using MediatR;
using Opdex.Platform.Domain.Models.Vaults;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.Certificates;

/// <summary>
/// Request to select vault certificate from indexed data.
/// </summary>
public class SelectVaultGovernanceCertificatesWithFilterQuery : IRequest<IEnumerable<VaultCertificate>>
{
    /// <summary>
    /// Creates a request to select vault certificates from indexed data.
    /// </summary>
    /// <param name="vaultId">Id of the vault.</param>
    /// <param name="cursor">Cursor filters.</param>
    public SelectVaultGovernanceCertificatesWithFilterQuery(ulong vaultId, VaultGovernanceCertificatesCursor cursor)
    {
        if (vaultId == 0) throw new ArgumentOutOfRangeException(nameof(vaultId), "Vault id must be greater than zero.");
        VaultId = vaultId;
        Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor), "Cursor must be set.");
    }

    public ulong VaultId { get; }
    public VaultGovernanceCertificatesCursor Cursor { get; }
}
