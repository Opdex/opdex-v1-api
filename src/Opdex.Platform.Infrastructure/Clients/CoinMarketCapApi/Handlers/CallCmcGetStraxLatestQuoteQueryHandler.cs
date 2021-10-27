using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CoinMarketCapApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CoinMarketCapApi.Queries;

namespace Opdex.Platform.Infrastructure.Clients.CoinMarketCapApi.Handlers
{
    public class CallCmcGetStraxLatestQuoteQueryHandler : IRequestHandler<CallCmcGetStraxLatestQuoteQuery, decimal>
    {
        private readonly IQuotesModule _quotesModule;
        private readonly ILogger<CallCmcGetStraxLatestQuoteQueryHandler> _logger;

        public CallCmcGetStraxLatestQuoteQueryHandler(IQuotesModule quotesModule, ILogger<CallCmcGetStraxLatestQuoteQueryHandler> logger)
        {
            _quotesModule = quotesModule ?? throw new ArgumentNullException(nameof(quotesModule));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<decimal> Handle(CallCmcGetStraxLatestQuoteQuery request, CancellationToken cancellationToken)
        {
            var quote = await _quotesModule.GetLatestQuoteAsync(CmcTokens.STRAX, cancellationToken);

            if (quote?.Data == null || !quote.Data.TryGetValue(CmcTokens.STRAX.ToString(), out var tokenDetails))
            {
                _logger.LogError($"STRAX quote not found at {DateTime.UtcNow}");
                return 0m;
            }

            var foundQuotePrice = tokenDetails.Quote.TryGetValue("USD", out var quotePrice);
            if (foundQuotePrice && quotePrice != null)
            {
                return quotePrice.Price;
            }

            _logger.LogError($"STRAX USD price not found at {DateTime.UtcNow}");
            return 0m;
        }
    }
}
