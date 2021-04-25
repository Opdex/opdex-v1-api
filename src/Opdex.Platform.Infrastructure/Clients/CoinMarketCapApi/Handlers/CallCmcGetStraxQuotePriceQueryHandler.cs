using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CoinMarketCapApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CoinMarketCapApi.Queries;

namespace Opdex.Platform.Infrastructure.Clients.CoinMarketCapApi.Handlers
{
    public class CallCmcGetStraxQuotePriceQueryHandler : IRequestHandler<CallCmcGetStraxQuotePriceQuery, decimal>
    {
        private readonly IQuotesModule _quotesModule;
        private readonly ILogger<CallCmcGetStraxQuotePriceQueryHandler> _logger;
        
        public CallCmcGetStraxQuotePriceQueryHandler(IQuotesModule quotesModule, 
            ILogger<CallCmcGetStraxQuotePriceQueryHandler> logger)
        {
            _quotesModule = quotesModule ?? throw new ArgumentNullException(nameof(quotesModule));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<decimal> Handle(CallCmcGetStraxQuotePriceQuery request, CancellationToken cancellationToken)
        {
            var quote = await _quotesModule.GetQuoteAsync(request.TokenId, cancellationToken);

            var found = quote.Data.TryGetValue(request.TokenId.ToString(), out var tokenDetails);
            if (!found || tokenDetails == null)
            {
                throw new Exception("Error getting CMC STRAX quote.");
            }

            var foundQuotePrice = tokenDetails.Quote.TryGetValue("USD", out var quotePrice);
            if (!foundQuotePrice || quotePrice == null)
            {
                throw new Exception("Error getting CMC STRAX quote price.");
            }

            return quotePrice.Price;
        }
    }
}