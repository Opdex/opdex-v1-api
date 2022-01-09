using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.VaultGovernances;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.VaultGovernances.ProposalCertificates;

public class MakeVaultProposalCertificateCommandHandler : IRequestHandler<MakeVaultProposalCertificateCommand, ulong>
{
    private readonly IMediator _mediator;

    public MakeVaultProposalCertificateCommandHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Task<ulong> Handle(MakeVaultProposalCertificateCommand request, CancellationToken cancellationToken)
    {
        return _mediator.Send(new PersistVaultProposalCertificateCommand(request.ProposalCertificate), cancellationToken);
    }
}
