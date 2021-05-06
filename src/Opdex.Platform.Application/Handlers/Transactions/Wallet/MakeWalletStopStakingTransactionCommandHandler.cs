using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Commands;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;

namespace Opdex.Platform.Application.Handlers.Transactions.Wallet
{
    public class MakeWalletStopStakingTransactionCommandHandler : IRequestHandler<MakeWalletStopStakingTransactionCommand, string>
    {
        private readonly IMediator _mediator;
        private const string MethodName = "Unstake";
        private const string CrsToSend = "0";
        
        public MakeWalletStopStakingTransactionCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<string> Handle(MakeWalletStopStakingTransactionCommand request, CancellationToken cancellationToken)
        {
            var parameters = new[]
            {
                request.Recipient.ToSmartContractParameter(SmartContractParameterType.Address), 
                request.Liquidate.ToSmartContractParameter(SmartContractParameterType.Boolean)
            };
            
            var callDto = new SmartContractCallRequestDto(request.LiquidityPool, request.WalletName, request.WalletAddress, 
                request.WalletPassword, CrsToSend, MethodName, parameters);
            
            return _mediator.Send(new CallCirrusCallSmartContractMethodCommand(callDto), cancellationToken);
        }
    }
}