using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Pools.SwapQuotes;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Pools.SwapQuotes
{
    public class CallCirrusGetAmountOutMultiHopQuoteQueryHandler : IRequestHandler<CallCirrusGetAmountOutMultiHopQuoteQuery, string>
    {
        private readonly ISmartContractsModule _smartContractsModule;
        private readonly ILogger<CallCirrusGetAmountOutMultiHopQuoteQueryHandler> _logger;
        
        public CallCirrusGetAmountOutMultiHopQuoteQueryHandler(ISmartContractsModule smartContractsModule, 
            ILogger<CallCirrusGetAmountOutMultiHopQuoteQueryHandler> logger)
        {
            _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string> Handle(CallCirrusGetAmountOutMultiHopQuoteQuery request, CancellationToken cancellationToken)
        {
            var quoteParams = new[] { 
                request.TokenInAmount.ToSmartContractParameter(SmartContractParameterType.UInt256),
                request.TokenInReserveCrs.ToSmartContractParameter(SmartContractParameterType.UInt256),
                request.TokenInReserveSrc.ToSmartContractParameter(SmartContractParameterType.UInt256),
                request.TokenOutReserveCrs.ToSmartContractParameter(SmartContractParameterType.UInt256),
                request.TokenOutReserveSrc.ToSmartContractParameter(SmartContractParameterType.UInt256)
            };
            
            var localCall = new LocalCallRequestDto(request.Market, request.Market, "GetAmountOut", quoteParams);
            
            var amountIn = await _smartContractsModule.LocalCallAsync(localCall, cancellationToken);

            if (amountIn.ErrorMessage != null)
            {
                throw new Exception($"Invalid request: {amountIn.ErrorMessage}");
            }
            
            return amountIn.Return.ToString();
        }
    }
}