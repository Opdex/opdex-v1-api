using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Commands;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;

namespace Opdex.Platform.Application.Handlers.Transactions.Wallet
{
    public class MakeWalletSkimTransactionCommandHandler : IRequestHandler<MakeWalletSkimTransactionCommand, string>
    {
        private readonly IMediator _mediator;
        private const string MethodName = "Skim";
        private const string CrsToSend = "0";

        public MakeWalletSkimTransactionCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<string> Handle(MakeWalletSkimTransactionCommand request, CancellationToken cancellationToken)
        {
            var parameters = new[]
            {
                request.Recipient.ToSmartContractParameter(SmartContractParameterType.Address)
            };

            var callDto = new SmartContractCallRequestDto(request.LiquidityPool, request.WalletName, request.WalletAddress,
                request.WalletPassword, CrsToSend, MethodName, parameters);

            return _mediator.Send(new CallCirrusCallSmartContractMethodCommand(callDto: callDto), cancellationToken);
        }
    }
}
