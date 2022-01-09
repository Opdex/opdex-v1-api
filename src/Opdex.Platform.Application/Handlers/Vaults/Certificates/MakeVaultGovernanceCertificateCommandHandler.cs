using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Vaults;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Vaults.Certificates;

public class MakeVaultGovernanceCertificateCommandHandler : IRequestHandler<MakeVaultGovernanceCertificateCommand, ulong>
{
    private readonly IMediator _mediator;

    public MakeVaultGovernanceCertificateCommandHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public Task<ulong> Handle(MakeVaultGovernanceCertificateCommand request, CancellationToken cancellationToken)
    {
        return _mediator.Send(new PersistVaultGovernanceCertificateCommand(request.Certificate), CancellationToken.None);
    }
}
