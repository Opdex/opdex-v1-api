using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Vaults;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Vaults;

/// <summary>
/// Call cirrus to get a vault contract proposal by proposalId at a specific block in time.
/// </summary>
public class CallCirrusGetVaultProposalSummaryByProposalIdQuery : IRequest<VaultProposalSummary>
{
    /// <summary>
    /// Constructor to create the call cirrus get vault contract proposal summary by proposalId.
    /// </summary>
    /// <param name="vault">The address of the vault to query.</param>
    /// <param name="proposalId">The Id of the proposal to look up.</param>
    /// <param name="blockHeight">The block height to get the vault proposal at.</param>
    public CallCirrusGetVaultProposalSummaryByProposalIdQuery(Address vault, ulong proposalId, ulong blockHeight)
    {
        Vault = vault != Address.Empty ? vault : throw new ArgumentNullException(nameof(vault), "Vault address must be provided.");
        ProposalId = proposalId > 0 ? proposalId : throw new ArgumentOutOfRangeException(nameof(proposalId), "ProposalId must be greater than zero.");
        BlockHeight = blockHeight > 0 ? blockHeight : throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
    }

    public Address Vault { get; }
    public ulong ProposalId { get; }
    public ulong BlockHeight { get; }
}
