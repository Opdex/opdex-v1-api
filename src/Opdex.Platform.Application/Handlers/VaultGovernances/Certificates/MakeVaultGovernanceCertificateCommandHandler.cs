using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.VaultGovernances;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.VaultGovernances.Certificates;

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
