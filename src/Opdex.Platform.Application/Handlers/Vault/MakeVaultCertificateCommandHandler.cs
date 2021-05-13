using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Vault;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Vault;

namespace Opdex.Platform.Application.Handlers.vault
{
    public class MakeVaultCertificateCommandHandler: IRequestHandler<MakeVaultCertificateCommand, bool>
    {
        private readonly IMediator _mediator;
        
        public MakeVaultCertificateCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public Task<bool> Handle(MakeVaultCertificateCommand request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new PersistVaultCertificateCommand(request.VaultCertificate), cancellationToken);
        }
    }
}