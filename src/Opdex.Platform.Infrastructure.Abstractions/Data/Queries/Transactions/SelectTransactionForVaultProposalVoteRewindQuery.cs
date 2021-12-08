using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Transactions;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Transactions;

public class SelectTransactionForVaultProposalVoteRewindQuery : IRequest<Transaction>
{
    public SelectTransactionForVaultProposalVoteRewindQuery(Address vault, Address pledger, ulong proposalPublicId)
    {
        Vault = vault != Address.Empty ? vault : throw new ArgumentNullException(nameof(vault), "Vault address must be provided.");
        Voter = pledger != Address.Empty ? pledger : throw new ArgumentNullException(nameof(pledger), "Voter address must be provided.");
        ProposalPublicId = proposalPublicId > 0 ? proposalPublicId : throw new ArgumentOutOfRangeException(nameof(proposalPublicId), "Proposal public Id must be greater than zero.");
    }

    public Address Vault { get; }
    public Address Voter { get; }
    public ulong ProposalPublicId { get; }
}
