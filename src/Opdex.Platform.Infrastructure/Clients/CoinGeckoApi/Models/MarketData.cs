using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Clients.CoinGeckoApi.Models;

public class MarketData
{
    public Dictionary<string, decimal?> CurrentPrice { get; set; }
}
