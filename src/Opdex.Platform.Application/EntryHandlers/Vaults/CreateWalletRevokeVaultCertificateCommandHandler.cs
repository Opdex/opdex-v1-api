using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Application.Abstractions.EntryCommands.Vaults;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Vaults
{
    public class CreateWalletRevokeVaultCertificateCommandHandler : IRequestHandler<CreateWalletRevokeVaultCertificateCommand, string>
    {
        private readonly IMediator _mediator;

        public CreateWalletRevokeVaultCertificateCommandHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task<string> Handle(CreateWalletRevokeVaultCertificateCommand request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new MakeRevokeVaultCertificateCommand(request.WalletAddress, request.Vault, request.Holder));
        }
    }
}
