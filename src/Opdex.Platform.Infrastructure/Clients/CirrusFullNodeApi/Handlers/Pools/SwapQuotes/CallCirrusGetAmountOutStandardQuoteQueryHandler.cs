using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Pools.SwapQuotes;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Pools.SwapQuotes
{
    public class CallCirrusGetAmountOutStandardQuoteQueryHandler 
        : IRequestHandler<CallCirrusGetAmountOutStandardQuoteQuery, string>
    {
        private readonly ISmartContractsModule _smartContractsModule;
        private readonly ILogger<CallCirrusGetAmountOutStandardQuoteQueryHandler> _logger;
        
        public CallCirrusGetAmountOutStandardQuoteQueryHandler(ISmartContractsModule smartContractsModule, 
            ILogger<CallCirrusGetAmountOutStandardQuoteQueryHandler> logger)
        {
            _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string> Handle(CallCirrusGetAmountOutStandardQuoteQuery request, CancellationToken cancellationToken)
        {
            var quoteParams = new[] { $"12#{request.AmountIn}", $"12#{request.ReserveIn}", $"12#{request.ReserveOut}" };
            var localCall = new LocalCallRequestDto(request.Market, request.Market, "GetAmountOut", quoteParams);
            var amountIn = await _smartContractsModule.LocalCallAsync(localCall, cancellationToken);

            return amountIn.Return.ToString();
        }
    }
}