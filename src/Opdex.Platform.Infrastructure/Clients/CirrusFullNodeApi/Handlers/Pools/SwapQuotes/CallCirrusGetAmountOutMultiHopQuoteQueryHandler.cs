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
            var quoteParams = new[] { $"12#{request.TokenInAmount}", $"12#{request.TokenInReserveCrs}", $"12#{request.TokenInReserveSrc}", 
                $"12#{request.TokenOutReserveCrs}", $"12#{request.TokenOutReserveSrc}" };
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