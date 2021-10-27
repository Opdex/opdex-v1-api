using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Common;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CoinMarketCapApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CoinMarketCapApi.Modules;
using Opdex.Platform.Infrastructure.Http;
using System;

namespace Opdex.Platform.Infrastructure.Clients.CoinMarketCapApi.Modules
{
    public class QuotesModule : ApiClientBase, IQuotesModule
    {
        public QuotesModule(HttpClient httpClient, ILogger<QuotesModule> logger) : base(httpClient, logger, Serialization.SnakeCasedJsonSettings)
        {
        }

        public Task<CmcLatestQuote> GetLatestQuoteAsync(int tokenId, CancellationToken cancellationToken)
        {
            var uri = string.Format(CmcUriHelper.Quotes.Latest, tokenId);
            return GetAsync<CmcLatestQuote>(uri, cancellationToken);
        }

        public Task<CmcHistoricalQuote> GetHistoricalQuoteAsync(int tokenId, DateTime dateTime, CancellationToken cancellationToken)
        {
            var uri = string.Format(CmcUriHelper.Quotes.Historical, tokenId, dateTime);
            return GetAsync<CmcHistoricalQuote>(uri, cancellationToken);
        }
    }
}
