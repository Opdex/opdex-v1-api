using MediatR;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.VaultGovernances;

/// <summary>
/// Call cirrus to get a vault contract proposal pledge by proposalId and pledger address at a specific block in time.
/// </summary>
public class CallCirrusGetVaultProposalPledgeByProposalIdAndPledgerQuery : IRequest<ulong>
{
    /// <summary>
    /// Constructor to create the call cirrus get vault contract proposal pledge by proposalId and pledger address.
    /// </summary>
    /// <param name="vault">The address of the vault to query.</param>
    /// <param name="proposalId">The Id of the proposal to look up.</param>
    /// <param name="pledger">The pledger's wallet address.</param>
    /// <param name="blockHeight">The block height to get the vault proposal at.</param>
    public CallCirrusGetVaultProposalPledgeByProposalIdAndPledgerQuery(Address vault, ulong proposalId, Address pledger, ulong blockHeight)
    {
        Vault = vault != Address.Empty ? vault : throw new ArgumentNullException(nameof(vault), "Vault address must be provided.");
        ProposalId = proposalId > 0 ? proposalId : throw new ArgumentOutOfRangeException(nameof(proposalId), "ProposalId must be greater than zero.");
        Pledger = pledger != Address.Empty ? pledger : throw new ArgumentNullException(nameof(pledger), "Pledger address must be provided.");
        BlockHeight = blockHeight > 0 ? blockHeight : throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
    }

    public Address Vault { get; }
    public ulong ProposalId { get; }
    public Address Pledger { get; }
    public ulong BlockHeight { get; }
}
