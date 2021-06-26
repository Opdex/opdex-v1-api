using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Commands;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Vaults
{
    public class MakeRedeemVaultCertificateCommandHandler : IRequestHandler<MakeRedeemVaultCertificateCommand, string>
    {
        private const string MethodName = "RedeemCertificates";
        private const string CrsToSend = "0";

        private readonly IMediator _mediator;

        public MakeRedeemVaultCertificateCommandHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task<string> Handle(MakeRedeemVaultCertificateCommand request, CancellationToken cancellationToken)
        {
            var call = new SmartContractCallRequestDto(request.Vault, request.WalletName, request.WalletAddress,
                                                       request.WalletPassword, CrsToSend, MethodName);

            return _mediator.Send(new CallCirrusCallSmartContractMethodCommand(call), cancellationToken);
        }
    }
}
