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
    public class MakeWalletCollectStakingRewardsTransactionCommandHandler
        : IRequestHandler<MakeWalletCollectStakingRewardsTransactionCommand, string>
    {
        private readonly IMediator _mediator;
        private const string MethodName = "CollectStakingRewards";
        private readonly FixedDecimal CrsToSend = FixedDecimal.Zero;

        public MakeWalletCollectStakingRewardsTransactionCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<string> Handle(MakeWalletCollectStakingRewardsTransactionCommand request, CancellationToken cancellationToken)
        {
            var parameters = new[]
            {
                request.Liquidate.ToSmartContractParameter(SmartContractParameterType.Boolean)
            };

            var callDto = new SmartContractCallRequestDto(request.LiquidityPool, request.WalletName, request.WalletAddress,
                                                          request.WalletPassword, CrsToSend, MethodName, parameters);

            return _mediator.Send(new CallCirrusCallSmartContractMethodCommand(callDto: callDto), cancellationToken);
        }
    }
}
