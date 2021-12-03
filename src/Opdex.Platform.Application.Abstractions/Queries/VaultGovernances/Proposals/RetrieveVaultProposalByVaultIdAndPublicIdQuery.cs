using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Proposals;

public class RetrieveVaultProposalByVaultIdAndPublicIdQuery : FindQuery<VaultProposal>
{
    public RetrieveVaultProposalByVaultIdAndPublicIdQuery(ulong vaultId, ulong publicId, bool findOrThrow = true) : base(findOrThrow)
    {
        VaultId = vaultId > 0 ? vaultId : throw new ArgumentOutOfRangeException(nameof(vaultId), "VaultId must be greater than zero.");
        PublicId = publicId > 0 ? publicId : throw new ArgumentOutOfRangeException(nameof(publicId), "PublicId must be greater than zero.");
    }

    public ulong VaultId { get; }
    public ulong PublicId { get; }
}
