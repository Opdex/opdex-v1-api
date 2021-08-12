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
    public class MakeWalletCreateVaultCertificateCommandHandler : IRequestHandler<MakeWalletCreateVaultCertificateCommand, string>
    {
        private const string MethodName = "CreateCertificate";
        private const string CrsToSend = "0";

        private readonly IMediator _mediator;

        public MakeWalletCreateVaultCertificateCommandHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task<string> Handle(MakeWalletCreateVaultCertificateCommand request, CancellationToken cancellationToken)
        {
            var parameters = new[]
            {
                request.Holder.ToSmartContractParameter(SmartContractParameterType.Address),
                request.Amount.ToSmartContractParameter(SmartContractParameterType.UInt256)
            };

            var call = new SmartContractCallRequestDto(request.Vault, request.WalletName, request.WalletAddress,
                                                       request.WalletPassword, CrsToSend, MethodName, parameters);

            return _mediator.Send(new CallCirrusCallSmartContractMethodCommand(callDto: call));
        }
    }
}
