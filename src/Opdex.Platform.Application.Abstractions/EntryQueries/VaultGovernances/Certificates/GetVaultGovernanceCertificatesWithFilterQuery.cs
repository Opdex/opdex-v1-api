using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Vaults;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances.Certificates;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.VaultGovernances.Certificates;

/// <summary>
/// Request to retrieve certificates in a vault.
/// </summary>
public class GetVaultGovernanceCertificatesWithFilterQuery : IRequest<VaultCertificatesDto>
{
    /// <summary>
    /// Creates a request to retrieve certificates in a vault.
    /// </summary>
    /// <param name="vault">Address of the vault.</param>
    /// <param name="cursor">Cursor filters.</param>
    public GetVaultGovernanceCertificatesWithFilterQuery(Address vault, VaultGovernanceCertificatesCursor cursor)
    {
        if (vault == Address.Empty) throw new ArgumentNullException(nameof(vault), "Vault address must be set.");
        Vault = vault;
        Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor), "Cursor must be set.");
    }

    public Address Vault { get; }
    public VaultGovernanceCertificatesCursor Cursor { get; }
}
