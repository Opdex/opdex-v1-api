using System.Threading;
using System.Threading.Tasks;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CoinMarketCapApi.Models;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CoinMarketCapApi.Modules;

public interface IQuotesModule
{
    Task<CmcLatestQuote> GetLatestQuoteAsync(int token, CancellationToken cancellationToken);
    Task<CmcHistoricalQuote> GetHistoricalQuoteAsync(int token, DateTime dateTime, CancellationToken cancellationToken);
}