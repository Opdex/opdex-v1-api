using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Vaults;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Vaults.ProposalCertificates;

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
