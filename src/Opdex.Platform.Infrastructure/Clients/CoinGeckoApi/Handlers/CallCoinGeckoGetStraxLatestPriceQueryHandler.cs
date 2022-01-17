using CoinGecko.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CoinGeckoApi.Queries;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Clients.CoinGeckoApi.Handlers;

public class CallCoinGeckoGetStraxLatestPriceQueryHandler : IRequestHandler<CallCoinGeckoGetStraxLatestPriceQuery, decimal>
{
    private readonly ICoinGeckoClient _client;
    private readonly ILogger<CallCoinGeckoGetStraxLatestPriceQueryHandler> _logger;

    public CallCoinGeckoGetStraxLatestPriceQueryHandler(ICoinGeckoClient client, ILogger<CallCoinGeckoGetStraxLatestPriceQueryHandler> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<decimal> Handle(CallCoinGeckoGetStraxLatestPriceQuery request, CancellationToken cancellationToken)
    {
        var result = await _client.CoinsClient.GetAllCoinDataWithId(CoinGeckoTokens.STRAX, "false", false, true, false, false, false);
        if ((result?.MarketData?.CurrentPrice?.TryGetValue(CoinGeckoTokens.USD, out var usdPrice) ?? false) && usdPrice.HasValue)
        {
            return usdPrice.Value;
        }

        _logger.LogError("STRAX quote not found");
        return 0m;
    }
}
