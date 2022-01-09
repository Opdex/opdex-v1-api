using MediatR;
using Opdex.Platform.Domain.Models.Vaults;
using System;

namespace Opdex.Platform.Application.Abstractions.Commands.Vaults;

public class MakeVaultProposalCertificateCommand : IRequest<ulong>
{
    public MakeVaultProposalCertificateCommand(VaultProposalCertificate proposalCertificate)
    {
        ProposalCertificate = proposalCertificate ?? throw new ArgumentNullException(nameof(proposalCertificate), "Proposal Certificate must be provided.");
    }

    public VaultProposalCertificate ProposalCertificate { get; }
}
