using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances.Proposals;

/// <summary>
/// Select a vault proposal by vault Id and the proposal's public Id from contract.
/// </summary>
public class SelectVaultProposalByVaultIdAndPublicIdQuery : FindQuery<VaultProposal>
{
    /// <summary>
    /// Constructor to create a select vault proposal by vault id and public id query.
    /// </summary>
    /// <param name="vaultId">The internal Id of the vault.</param>
    /// <param name="publicId">The proposal's public Id as set in contract.</param>
    /// <param name="findOrThrow">Find or throw, defaults to true, when true throws not found exception if no record is found.</param>
    public SelectVaultProposalByVaultIdAndPublicIdQuery(ulong vaultId, ulong publicId, bool findOrThrow = true) : base(findOrThrow)
    {
        VaultId = vaultId > 0 ? vaultId : throw new ArgumentOutOfRangeException(nameof(vaultId), "VaultId must be greater than zero.");
        PublicId = publicId > 0 ? publicId : throw new ArgumentOutOfRangeException(nameof(publicId), "PublicId must be greater than zero.");
    }

    public ulong VaultId { get; }
    public ulong PublicId { get; }
}
