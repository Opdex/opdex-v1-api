namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.VaultGovernances;

public class VaultProposalCertificateEntity : AuditEntity
{
    public ulong Id { get; set; }
    public ulong ProposalId { get; set; }
    public ulong CertificateId { get; set; }
}
