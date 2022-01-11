using MediatR;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.Certificates;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Queries.Vaults.Certificates;

/// <summary>
/// Request to retrieve certificates in a vault.
/// </summary>
public class RetrieveVaultCertificatesWithFilterQuery : IRequest<IEnumerable<VaultCertificate>>
{
    /// <summary>
    /// Creates a request to retrieve certificates in a vault.
    /// </summary>
    /// <param name="vaultId">Id of the vault.</param>
    /// <param name="cursor">Cursor filters.</param>
    public RetrieveVaultCertificatesWithFilterQuery(ulong vaultId, VaultCertificatesCursor cursor)
    {
        if (vaultId == 0) throw new ArgumentOutOfRangeException(nameof(vaultId), "Vault id must be greater than zero.");
        VaultId = vaultId;
        Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor), "Cursor must be set.");
    }

    public ulong VaultId { get; }
    public VaultCertificatesCursor Cursor { get; }
}
