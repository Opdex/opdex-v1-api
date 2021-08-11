using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Commands;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Vaults
{
    public class MakeWalletRevokeVaultCertificateCommandHandler : IRequestHandler<MakeWalletRevokeVaultCertificateCommand, string>
    {
        private const string MethodName = "RevokeCertificates";
        private const string CrsToSend = "0";

        private readonly IMediator _mediator;

        public MakeWalletRevokeVaultCertificateCommandHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task<string> Handle(MakeWalletRevokeVaultCertificateCommand request, CancellationToken cancellationToken)
        {
            var parameters = new[]
            {
                request.Holder.ToSmartContractParameter(SmartContractParameterType.Address)
            };

            var call = new SmartContractCallRequestDto(request.Vault, request.WalletName, request.WalletAddress,
                                                       request.WalletPassword, CrsToSend, MethodName, parameters);

            return _mediator.Send(new CallCirrusCallSmartContractMethodCommand(call));
        }
    }
}
