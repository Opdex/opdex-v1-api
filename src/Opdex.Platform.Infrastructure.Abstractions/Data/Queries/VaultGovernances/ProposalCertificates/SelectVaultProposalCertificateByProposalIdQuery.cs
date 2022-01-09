using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances.ProposalCertificates;

public class SelectVaultProposalCertificateByProposalIdQuery : FindQuery<VaultProposalCertificate>
{
    public SelectVaultProposalCertificateByProposalIdQuery(ulong proposalId, bool findOrThrow = true) : base(findOrThrow)
    {
        ProposalId = proposalId > 0 ? proposalId : throw new ArgumentOutOfRangeException(nameof(proposalId), "Proposal Id must be greater than zero.");
    }

    public ulong ProposalId { get; }
}
