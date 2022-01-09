using MediatR;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.VaultGovernances;

public class PersistVaultProposalCertificateCommand : IRequest<ulong>
{
    public PersistVaultProposalCertificateCommand(VaultProposalCertificate proposalCertificate)
    {
        ProposalCertificate = proposalCertificate ?? throw new ArgumentNullException(nameof(proposalCertificate), "Proposal Certificate must be provided.");
    }

    public VaultProposalCertificate ProposalCertificate { get; }
}
