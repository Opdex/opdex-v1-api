using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Commands;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;

namespace Opdex.Platform.Application.Handlers.Transactions.Wallet
{
    public class MakeWalletRewardMiningPoolsTransactionCommandHandler
        : IRequestHandler<MakeWalletRewardMiningPoolsTransactionCommand, string>
    {
        private readonly IMediator _mediator;
        private const string MethodName = "RewardMiningPools";
        private const string CrsToSend = "0";
        
        public MakeWalletRewardMiningPoolsTransactionCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<string> Handle(MakeWalletRewardMiningPoolsTransactionCommand request, CancellationToken cancellationToken)
        {
            var callDto = new SmartContractCallRequestDto(request.Governance, request.WalletName, request.WalletAddress, 
                request.WalletPassword, CrsToSend, MethodName, new string[0]);
            
            return _mediator.Send(new CallCirrusCallSmartContractMethodCommand(callDto), cancellationToken);
        }
    }
}