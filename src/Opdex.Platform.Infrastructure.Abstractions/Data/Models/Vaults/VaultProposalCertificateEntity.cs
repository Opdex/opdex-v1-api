namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.Vaults;

public class VaultProposalCertificateEntity
{
    public ulong Id { get; set; }
    public ulong ProposalId { get; set; }
    public ulong CertificateId { get; set; }
}
