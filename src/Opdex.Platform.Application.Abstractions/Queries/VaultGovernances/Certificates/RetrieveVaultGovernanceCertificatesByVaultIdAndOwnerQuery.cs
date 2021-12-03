using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.VaultGovernances;
using Opdex.Platform.Domain.Models.Vaults;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Certificates;

public class RetrieveVaultGovernanceCertificatesByVaultIdAndOwnerQuery : IRequest<IEnumerable<VaultCertificate>>
{
    public RetrieveVaultGovernanceCertificatesByVaultIdAndOwnerQuery(ulong vaultId, Address owner)
    {
        VaultId = vaultId > 0 ? vaultId : throw new ArgumentOutOfRangeException(nameof(vaultId), "VaultId must be greater than zero.");
        Owner = owner != Address.Empty ? owner : throw new ArgumentNullException(nameof(owner), "Owner address must be provided.");
    }

    public ulong VaultId { get; }
    public Address Owner { get; }
}
