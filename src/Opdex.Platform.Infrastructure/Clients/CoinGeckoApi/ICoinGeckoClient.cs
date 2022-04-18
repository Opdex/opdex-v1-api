namespace Opdex.Platform.Infrastructure.Clients.CoinGeckoApi;

public interface ICoinGeckoClient
{
    ICoinsClient CoinsClient { get; }
}
