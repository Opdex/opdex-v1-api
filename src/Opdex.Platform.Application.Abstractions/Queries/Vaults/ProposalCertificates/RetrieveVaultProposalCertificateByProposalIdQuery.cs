using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Vaults;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries.Vaults.ProposalCertificates;

public class RetrieveVaultProposalCertificateByProposalIdQuery : FindQuery<VaultProposalCertificate>
{
    public RetrieveVaultProposalCertificateByProposalIdQuery(ulong proposalId, bool findOrThrow = true) : base(findOrThrow)
    {
        ProposalId = proposalId > 0 ? proposalId : throw new ArgumentOutOfRangeException(nameof(proposalId), "Proposal Id must be greater than zero.");
    }

    public ulong ProposalId { get; }
}
