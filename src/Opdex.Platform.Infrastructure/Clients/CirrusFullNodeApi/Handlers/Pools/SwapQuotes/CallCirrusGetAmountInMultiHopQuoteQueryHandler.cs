using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Pools.SwapQuotes;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Pools.SwapQuotes
{
    public class CallCirrusGetAmountInMultiHopQuoteQueryHandler : IRequestHandler<CallCirrusGetAmountInMultiHopQuoteQuery, string>
    {
        private readonly ISmartContractsModule _smartContractsModule;
        private readonly ILogger<CallCirrusGetAmountInMultiHopQuoteQueryHandler> _logger;
        
        public CallCirrusGetAmountInMultiHopQuoteQueryHandler(ISmartContractsModule smartContractsModule, 
            ILogger<CallCirrusGetAmountInMultiHopQuoteQueryHandler> logger)
        {
            _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string> Handle(CallCirrusGetAmountInMultiHopQuoteQuery request, CancellationToken cancellationToken)
        {
            var quoteParams = new[]
            {
                request.TokenOutAmount.ToSmartContractParameter(SmartContractParameterType.UInt256),
                request.TokenOutReserveCrs.ToSmartContractParameter(SmartContractParameterType.UInt256),
                request.TokenOutReserveSrc.ToSmartContractParameter(SmartContractParameterType.UInt256),
                request.TokenInReserveCrs.ToSmartContractParameter(SmartContractParameterType.UInt256),
                request.TokenInReserveSrc.ToSmartContractParameter(SmartContractParameterType.UInt256)
            };
            
            var localCall = new LocalCallRequestDto(request.Market, request.Market, "GetAmountIn", quoteParams);
            
            var amountIn = await _smartContractsModule.LocalCallAsync(localCall, cancellationToken);

            if (amountIn.ErrorMessage != null)
            {
                throw new Exception($"Invalid request: {amountIn.ErrorMessage}");
            }
            
            return amountIn.Return.ToString();
        }
    }
}