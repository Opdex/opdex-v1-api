using CoinGecko.Clients;
using CoinGecko.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CoinGeckoApi.Queries;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Clients.CoinGeckoApi.Handlers;

public class CallCoinGeckoGetStraxHistoricalPriceQueryHandler : IRequestHandler<CallCoinGeckoGetStraxHistoricalPriceQuery, decimal>
{
    private readonly ICoinGeckoClient _client;
    private readonly ILogger<CallCoinGeckoGetStraxHistoricalPriceQueryHandler> _logger;

    public CallCoinGeckoGetStraxHistoricalPriceQueryHandler(ICoinGeckoClient client, ILogger<CallCoinGeckoGetStraxHistoricalPriceQueryHandler> logger)
    {
        _client = client;
        _logger = logger;
    }

    /*
     * We retrieve market chart data for more granularity. According to CoinGecko API reference:
            Data granularity is automatic (cannot be adjusted)
              * 1 day from query time = 5 minute interval data
              * 1 - 90 days from query time = hourly data
              *  above 90 days from query time = daily data (00:00 UTC)
     */
    public async Task<decimal> Handle(CallCoinGeckoGetStraxHistoricalPriceQuery request, CancellationToken cancellationToken)
    {
        int hoursDiff = DateTime.UtcNow - request.DateTime > TimeSpan.FromDays(90) ? 24 : 1;
        var from = new DateTimeOffset(request.DateTime.AddHours(-hoursDiff).ToUniversalTime()).ToUnixTimeSeconds().ToString();
        var to = new DateTimeOffset(request.DateTime.AddHours(hoursDiff).ToUniversalTime()).ToUnixTimeSeconds().ToString();
        var marketData = await _client.CoinsClient.GetMarketChartRangeByCoinId(CoinGeckoTokens.STRAX, CoinGeckoTokens.USD, from, to);

        if ((marketData?.Prices?.Length ?? 0) == 0)
        {
            _logger.LogError("STRAX quote not found for {RequestedTime}", request.DateTime);
            return 0m;
        }

        // coingecko accepts unix time in seconds, returns unix time in milliseconds 🧐
        var requestedTimeMs = new DateTimeOffset(request.DateTime.ToUniversalTime()).ToUnixTimeMilliseconds();

        // we select the time difference between the requested time and snapshot, returning the price closest to requested time
        var closestPrice = marketData!.Prices!.Select(item => (Math.Abs(requestedTimeMs - item[0]!.Value), item[1]))
                                     .MinBy(item => item.Item1)
                                     .Item2;
        return closestPrice!.Value;
    }
}
