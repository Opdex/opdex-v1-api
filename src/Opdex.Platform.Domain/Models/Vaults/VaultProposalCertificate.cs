using System;

namespace Opdex.Platform.Domain.Models.Vaults;

public class VaultProposalCertificate
{
    public VaultProposalCertificate(ulong proposalId, ulong certificateId)
    {
        ProposalId = proposalId > 0 ? proposalId : throw new ArgumentOutOfRangeException(nameof(proposalId), "Proposal Id must be greater than zero.");
        CertificateId = certificateId > 0 ? certificateId : throw new ArgumentOutOfRangeException(nameof(certificateId), "Certificate Id must be greater than zero.");
    }

    public VaultProposalCertificate(ulong id, ulong proposalId, ulong certificateId)
    {
        Id = id;
        ProposalId = proposalId;
        CertificateId = certificateId;
    }

    public ulong Id { get; }
    public ulong ProposalId { get; }
    public ulong CertificateId { get; }
}
