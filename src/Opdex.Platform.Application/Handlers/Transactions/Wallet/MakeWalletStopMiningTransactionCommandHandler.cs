using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Commands;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;

namespace Opdex.Platform.Application.Handlers.Transactions.Wallet
{
    public class MakeWalletStopMiningTransactionCommandHandler
        : IRequestHandler<MakeWalletStopMiningTransactionCommand, string>
    {
        private readonly IMediator _mediator;
        private const string MethodName = "StopMining";
        private readonly FixedDecimal CrsToSend = FixedDecimal.Zero;

        public MakeWalletStopMiningTransactionCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<string> Handle(MakeWalletStopMiningTransactionCommand request, CancellationToken cancellationToken)
        {
            var miningParameters = new[]
            {
                request.Amount.ToSmartContractParameter(SmartContractParameterType.UInt256)
            };

            var callDto = new SmartContractCallRequestDto(request.MiningPool, request.WalletName, request.WalletAddress,
                request.WalletPassword, CrsToSend, MethodName, miningParameters);

            return _mediator.Send(new CallCirrusCallSmartContractMethodCommand(callDto: callDto), cancellationToken);
        }
    }
}
