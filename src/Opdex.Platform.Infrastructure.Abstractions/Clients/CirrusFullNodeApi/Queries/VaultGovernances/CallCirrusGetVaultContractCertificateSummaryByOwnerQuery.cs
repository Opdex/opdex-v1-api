using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Vaults;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.VaultGovernances;

public class CallCirrusGetVaultGovernanceContractCertificateSummaryByOwnerQuery : IRequest<VaultContractCertificateSummary>
{
    public CallCirrusGetVaultGovernanceContractCertificateSummaryByOwnerQuery(Address vault, Address owner, ulong blockHeight)
    {
        if (vault == Address.Empty)
        {
            throw new ArgumentNullException(nameof(vault), "Vault address must be provided.");
        }

        if (owner == Address.Empty)
        {
            throw new ArgumentNullException(nameof(owner), "Owner address must be provided.");
        }

        if (blockHeight == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
        }

        Vault = vault;
        Owner = owner;
        BlockHeight = blockHeight;
    }

    public Address Vault { get; }
    public Address Owner { get; }
    public ulong BlockHeight { get; }
}
