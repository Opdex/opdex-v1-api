using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Commands;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;

namespace Opdex.Platform.Application.Handlers.Transactions.Wallet
{
    public class MakeWalletCollectMiningRewardsTransactionCommandHandler
        : IRequestHandler<MakeWalletCollectMiningRewardsTransactionCommand, string>
    {
        private readonly IMediator _mediator;
        private const string MethodName = "CollectMiningRewards";
        private const string CrsToSend = "0";

        public MakeWalletCollectMiningRewardsTransactionCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<string> Handle(MakeWalletCollectMiningRewardsTransactionCommand request, CancellationToken cancellationToken)
        {
            var callDto = new SmartContractCallRequestDto(request.MiningPool, request.WalletName, request.WalletAddress,
                request.WalletPassword, CrsToSend, MethodName, new string[0]);

            return _mediator.Send(new CallCirrusCallSmartContractMethodCommand(callDto: callDto), cancellationToken);
        }
    }
}
