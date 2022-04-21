namespace Opdex.Platform.Infrastructure.Clients.CoinGeckoApi;

public class CoinGeckoCoin
{
    public static readonly CoinGeckoCoin Strax = new("stratis");
    public static readonly CoinGeckoCoin Usd = new("usd");

    private CoinGeckoCoin(string name)
    {
        Name = name;
    }

    public string Name { get; }
}
