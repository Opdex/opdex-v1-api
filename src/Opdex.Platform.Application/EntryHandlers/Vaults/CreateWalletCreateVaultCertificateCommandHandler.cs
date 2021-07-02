using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Application.Abstractions.EntryCommands.Vaults;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Extensions;
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
            return _mediator.Send(new MakeWalletCreateVaultCertificateCommand(request.WalletAddress,
                                                                              request.Vault,
                                                                              request.Holder,
                                                                              request.Amount.ToSatoshis(TokenConstants.Opdex.Decimals)));
        }
    }
}
