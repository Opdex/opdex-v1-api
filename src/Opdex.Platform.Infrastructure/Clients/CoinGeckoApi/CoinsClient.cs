using Opdex.Platform.Infrastructure.Clients.CoinGeckoApi.Models;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Clients.CoinGeckoApi;

public class CoinsClient : ICoinsClient
{
    private readonly HttpClient _httpClient;

    public CoinsClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<FullCoinDataById> GetAllCoinDataWithId(CoinGeckoCoin id, bool localization, bool tickers,
                                                       bool marketData, bool communityData, bool developerData,
                                                       bool sparkline, CancellationToken cancellationToken = default)
    {
        var requestUri = $"coins/{id.Name}?localization={localization}&tickers={tickers}&market_data={marketData}&community_data={communityData}&developer_data={developerData}&sparkline={sparkline}";
        return await _httpClient.GetFromJsonAsync<FullCoinDataById>(requestUri, CoinGeckoClient.SerializationOptions, cancellationToken);
    }

    public async Task<MarketChartById> GetMarketChartRangeByCoinId(CoinGeckoCoin id, CoinGeckoCoin vsCurrency,
                                                                   DateTimeOffset fromDate, DateTimeOffset toDate,
                                                                   CancellationToken cancellationToken = default)
    {
        var from = fromDate.ToUniversalTime().ToUnixTimeSeconds().ToString();
        var to = toDate.ToUniversalTime().ToUnixTimeSeconds().ToString();
        var requestUri = $"coins/{id.Name}/market_chart/range?vs_currency={vsCurrency.Name}&from={from}&to={to}";
        return await _httpClient.GetFromJsonAsync<MarketChartById>(requestUri, CoinGeckoClient.SerializationOptions, cancellationToken);
    }
}
