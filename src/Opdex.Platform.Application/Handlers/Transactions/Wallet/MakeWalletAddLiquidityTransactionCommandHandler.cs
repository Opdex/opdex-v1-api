using System;
using System.Collections.Generic;
using System.Globalization;
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
    public class MakeWalletAddLiquidityTransactionCommandHandler : IRequestHandler<MakeWalletAddLiquidityTransactionCommand, string>
    {
        private readonly IMediator _mediator;
        
        public MakeWalletAddLiquidityTransactionCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<string> Handle(MakeWalletAddLiquidityTransactionCommand request, CancellationToken cancellationToken)
        {
            const string methodName = "AddLiquidity";
            
            // Todo: Convert to decimals
            var crsToSend = Math.Round((decimal)request.AmountCrsDesired / 100_000_000, 8).ToString();
            
            var parameters = new []
            {
                request.Token.ToSmartContractParameter(SmartContractParameterType.Address),
                request.AmountSrcDesired.ToSmartContractParameter(SmartContractParameterType.UInt256),
                request.AmountCrsMin.ToSmartContractParameter(SmartContractParameterType.UInt64),
                request.AmountSrcMin.ToSmartContractParameter(SmartContractParameterType.UInt256),
                request.To.ToSmartContractParameter(SmartContractParameterType.Address),
                0.ToSmartContractParameter(SmartContractParameterType.UInt64)
            };
            
            var callDto = new SmartContractCallRequestDto(request.Market, request.To, crsToSend, methodName, parameters);
            
            return _mediator.Send(new CallCirrusCallSmartContractMethodCommand(callDto), cancellationToken);
        }
    }
}