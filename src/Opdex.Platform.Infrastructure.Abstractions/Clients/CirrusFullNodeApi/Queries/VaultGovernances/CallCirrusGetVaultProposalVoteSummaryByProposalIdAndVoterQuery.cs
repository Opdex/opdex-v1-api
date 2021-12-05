using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.VaultGovernances;

/// <summary>
/// Call cirrus to get a vault contract proposal vote by proposalId and voter at a specific block in time.
/// </summary>
public class CallCirrusGetVaultProposalVoteSummaryByProposalIdAndVoterQuery : IRequest<VaultProposalVoteSummary>
{
    /// <summary>
    /// Constructor to create the call cirrus get vault contract proposal vote summary by proposalId and voter.
    /// </summary>
    /// <param name="vault">The address of the vault to query.</param>
    /// <param name="proposalId">The Id of the proposal to look up.</param>
    /// <param name="voter">The address of the voter.</param>
    /// <param name="blockHeight">The block height to get the vault proposal at.</param>
    public CallCirrusGetVaultProposalVoteSummaryByProposalIdAndVoterQuery(Address vault, ulong proposalId, Address voter, ulong blockHeight)
    {
        Vault = vault != Address.Empty ? vault : throw new ArgumentNullException(nameof(vault), "Vault address must be provided.");
        ProposalId = proposalId > 0 ? proposalId : throw new ArgumentOutOfRangeException(nameof(proposalId), "ProposalId must be greater than zero.");
        Voter = voter != Address.Empty ? voter : throw new ArgumentNullException(nameof(voter), "Voter address must be provided.");
        BlockHeight = blockHeight > 0 ? blockHeight : throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
    }

    public Address Vault { get; }
    public ulong ProposalId { get; }
    public Address Voter { get; }
    public ulong BlockHeight { get; }
}
