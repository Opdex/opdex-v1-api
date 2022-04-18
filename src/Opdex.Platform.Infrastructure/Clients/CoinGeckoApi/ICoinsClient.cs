using Opdex.Platform.Infrastructure.Clients.CoinGeckoApi.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Clients.CoinGeckoApi;

public interface ICoinsClient
{
    Task<FullCoinDataById> GetAllCoinDataWithId(CoinGeckoCoin id, bool localization, bool tickers, bool marketData,
        bool communityData, bool developerData, bool sparkline, CancellationToken cancellationToken = default);

    Task<MarketChartById> GetMarketChartRangeByCoinId(CoinGeckoCoin id, CoinGeckoCoin vsCurrency,
        DateTimeOffset fromDate, DateTimeOffset toDate, CancellationToken cancellationToken = default);
}
