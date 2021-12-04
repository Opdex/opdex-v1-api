using MediatR;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.VaultGovernances;

public class CallCirrusGetVaultProposalPledgeByProposalIdAndPledgerQuery : IRequest<ulong>
{
    public CallCirrusGetVaultProposalPledgeByProposalIdAndPledgerQuery(Address vault, ulong proposalId, Address pledger, ulong blockHeight)
    {
        Vault = vault != Address.Empty ? vault : throw new ArgumentNullException(nameof(vault), "Vault address must be provided");
        ProposalId = proposalId > 0 ? proposalId : throw new ArgumentOutOfRangeException(nameof(proposalId), "ProposalId must be greater than zero.");
        Pledger = pledger != Address.Empty ? pledger : throw new ArgumentNullException(nameof(pledger), "Pledger address must be provided");
        BlockHeight = blockHeight > 0 ? blockHeight : throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
    }

    public Address Vault { get; }
    public ulong ProposalId { get; }
    public Address Pledger { get; }
    public ulong BlockHeight { get; }
}
