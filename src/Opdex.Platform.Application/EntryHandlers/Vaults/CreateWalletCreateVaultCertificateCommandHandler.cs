using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Application.Abstractions.EntryCommands.Vaults;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Vaults
{
    public class CreateWalletCreateVaultCertificateCommandHandler : IRequestHandler<CreateWalletCreateVaultCertificateCommand, string>
    {
        private readonly IMediator _mediator;

        public CreateWalletCreateVaultCertificateCommandHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task<string> Handle(CreateWalletCreateVaultCertificateCommand request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new MakeCreateVaultCertificateCommand(request.WalletAddress,
                                                                        request.Vault,
                                                                        request.Holder,
                                                                        request.Amount));
        }
    }
}
