using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Commands;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Vaults
{
    public class MakeWalletSetVaultOwnerCommandHandler : IRequestHandler<MakeWalletSetVaultOwnerCommand, string>
    {
        private const string MethodName = "SetOwner";
        private const string CrsToSend = "0";
        private readonly IMediator _mediator;

        public MakeWalletSetVaultOwnerCommandHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task<string> Handle(MakeWalletSetVaultOwnerCommand request, CancellationToken cancellationToken)
        {
            var parameters = new[]
            {
                request.Owner.ToSmartContractParameter(SmartContractParameterType.Address)
            };

            var call = new SmartContractCallRequestDto(request.Vault, request.WalletName, request.WalletAddress,
                                                       request.WalletPassword, CrsToSend, MethodName, parameters);

            return _mediator.Send(new CallCirrusCallSmartContractMethodCommand(call));
        }
    }
}
