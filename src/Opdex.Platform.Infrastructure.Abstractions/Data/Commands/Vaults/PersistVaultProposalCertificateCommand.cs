using MediatR;
using Opdex.Platform.Domain.Models.Vaults;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Vaults;

public class PersistVaultProposalCertificateCommand : IRequest<ulong>
{
    public PersistVaultProposalCertificateCommand(VaultProposalCertificate proposalCertificate)
    {
        ProposalCertificate = proposalCertificate ?? throw new ArgumentNullException(nameof(proposalCertificate), "Proposal Certificate must be provided.");
    }

    public VaultProposalCertificate ProposalCertificate { get; }
}
