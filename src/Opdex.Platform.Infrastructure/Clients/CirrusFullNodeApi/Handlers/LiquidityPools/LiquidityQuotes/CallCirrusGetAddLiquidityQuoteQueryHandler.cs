using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.LiquidityPools.LiquidityQuotes;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.LiquidityPools.LiquidityQuotes
{
    public class CallCirrusGetAddLiquidityQuoteQueryHandler : IRequestHandler<CallCirrusGetAddLiquidityQuoteQuery, string>
    {
        private readonly ISmartContractsModule _smartContractsModule;
        private readonly ILogger<CallCirrusGetAddLiquidityQuoteQueryHandler> _logger;

        public CallCirrusGetAddLiquidityQuoteQueryHandler(ISmartContractsModule smartContractsModule,
            ILogger<CallCirrusGetAddLiquidityQuoteQueryHandler> logger)
        {
            _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string> Handle(CallCirrusGetAddLiquidityQuoteQuery request, CancellationToken cancellationToken)
        {
            var quoteParams = new[] { $"12#{request.AmountA}", $"12#{request.ReserveA}", $"12#{request.ReserveB}" };
            var localCall = new LocalCallRequestDto(request.Market, request.Market, "GetLiquidityQuote", quoteParams);
            var amountIn = await _smartContractsModule.LocalCallAsync(localCall, cancellationToken);

            if (amountIn.ErrorMessage != null)
            {
                throw new Exception($"Invalid request: {amountIn.ErrorMessage}");
            }

            return amountIn.Return.ToString();
        }
    }
}
