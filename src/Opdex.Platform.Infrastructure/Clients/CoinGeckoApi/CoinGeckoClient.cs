using O9d.Json.Formatting;
using System;
using System.Net.Http;
using System.Text.Json;

namespace Opdex.Platform.Infrastructure.Clients.CoinGeckoApi;

public class CoinGeckoClient : ICoinGeckoClient
{
    public static readonly JsonSerializerOptions SerializationOptions = new()
    {
        PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy()
    };

    public CoinGeckoClient(HttpClient httpClient)
    {
        if (httpClient is null) throw new ArgumentNullException(nameof(httpClient));
        httpClient.BaseAddress = new Uri("https://api.coingecko.com/api/v3/");
        CoinsClient = new CoinsClient(httpClient);
    }

    public ICoinsClient CoinsClient { get; }
}
