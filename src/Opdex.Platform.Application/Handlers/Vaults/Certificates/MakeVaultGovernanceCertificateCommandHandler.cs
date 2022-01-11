using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Vaults;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Vaults.Certificates;

public class MakeVaultCertificateCommandHandler : IRequestHandler<MakeVaultCertificateCommand, ulong>
{
    private readonly IMediator _mediator;

    public MakeVaultCertificateCommandHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public Task<ulong> Handle(MakeVaultCertificateCommand request, CancellationToken cancellationToken)
    {
        return _mediator.Send(new PersistVaultCertificateCommand(request.Certificate), CancellationToken.None);
    }
}
