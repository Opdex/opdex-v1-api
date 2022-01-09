using MediatR;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System;

namespace Opdex.Platform.Application.Abstractions.Commands.VaultGovernances;

public class MakeVaultProposalCertificateCommand : IRequest<ulong>
{
    public MakeVaultProposalCertificateCommand(VaultProposalCertificate proposalCertificate)
    {
        ProposalCertificate = proposalCertificate ?? throw new ArgumentNullException(nameof(proposalCertificate), "Proposal Certificate must be provided.");
    }

    public VaultProposalCertificate ProposalCertificate { get; }
}
