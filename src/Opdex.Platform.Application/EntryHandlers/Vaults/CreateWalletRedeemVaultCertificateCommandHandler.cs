using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Application.Abstractions.EntryCommands.Vaults;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Vaults
{
    public class CreateWalletRedeemVaultCertificateCommandHandler : IRequestHandler<CreateWalletRedeemVaultCertificateCommand, string>
    {
        private readonly IMediator _mediator;

        public CreateWalletRedeemVaultCertificateCommandHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task<string> Handle(CreateWalletRedeemVaultCertificateCommand request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new MakeWalletRedeemVaultCertificateCommand(request.WalletAddress, request.Vault));
        }
    }
}
