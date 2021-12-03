using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CoinMarketCapApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CoinMarketCapApi.Queries;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Clients.CoinMarketCapApi.Handlers;

public class CallCmcGetStraxHistoricalQuoteQueryHandler : IRequestHandler<CallCmcGetStraxHistoricalQuoteQuery, decimal>
{
    private readonly IQuotesModule _quotesModule;
    private readonly ILogger<CallCmcGetStraxHistoricalQuoteQueryHandler> _logger;

    public CallCmcGetStraxHistoricalQuoteQueryHandler(IQuotesModule quotesModule, ILogger<CallCmcGetStraxHistoricalQuoteQueryHandler> logger)
    {
        _quotesModule = quotesModule ?? throw new ArgumentNullException(nameof(quotesModule));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<decimal> Handle(CallCmcGetStraxHistoricalQuoteQuery request, CancellationToken cancellationToken)
    {
        var quote = await _quotesModule.GetHistoricalQuoteAsync(CmcTokens.STRAX, request.DateTime, cancellationToken);

        var fiatPrices = quote?.Data?.Quotes?.FirstOrDefault()?.Quote;

        if (fiatPrices == null)
        {
            _logger.LogError($"STRAX quote not found for {request.DateTime}");
            return 0m;
        }

        var usdFound = fiatPrices.TryGetValue("USD", out var usd);
        if (usdFound && usd != null)
        {
            return usd.Price;
        }

        _logger.LogError($"STRAX USD price not found for {request.DateTime}");
        return 0m;
    }
}