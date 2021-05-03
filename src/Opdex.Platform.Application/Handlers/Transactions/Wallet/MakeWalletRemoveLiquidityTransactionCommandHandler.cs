using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Options;
using Opdex.Platform.Common;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Commands;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi;

namespace Opdex.Platform.Application.Handlers.Transactions.Wallet
{
    public class MakeWalletRemoveLiquidityTransactionCommandHandler : IRequestHandler<MakeWalletRemoveLiquidityTransactionCommand, string>
    {
        private readonly IMediator _mediator;
        
        public MakeWalletRemoveLiquidityTransactionCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<string> Handle(MakeWalletRemoveLiquidityTransactionCommand request, CancellationToken cancellationToken)
        {
            const string methodName = "RemoveLiquidity";
            const string amountToSend = "0";
            
            var parameters = new []
            {
                request.Token.ToSmartContractParameter(SmartContractParameterType.Address),
                request.Liquidity.ToSmartContractParameter(SmartContractParameterType.UInt256),
                request.AmountCrsMin.ToSmartContractParameter(SmartContractParameterType.UInt64),
                request.AmountSrcMin.ToSmartContractParameter(SmartContractParameterType.UInt256),
                request.To.ToSmartContractParameter(SmartContractParameterType.Address),
                0.ToSmartContractParameter(SmartContractParameterType.UInt64)
            };
            
            var callDto = new SmartContractCallRequestDto(request.Market, request.To, amountToSend, methodName, parameters);
            
            return _mediator.Send(new CallCirrusCallSmartContractMethodCommand(callDto), cancellationToken);
        }
    }
}