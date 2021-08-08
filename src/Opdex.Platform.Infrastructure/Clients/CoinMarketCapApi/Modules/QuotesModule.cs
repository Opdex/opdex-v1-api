using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CoinMarketCapApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CoinMarketCapApi.Modules;
using Opdex.Platform.Infrastructure.Http;

namespace Opdex.Platform.Infrastructure.Clients.CoinMarketCapApi.Modules
{
    public class QuotesModule : ApiClientBase, IQuotesModule
    {
        public QuotesModule(HttpClient httpClient, ILogger<QuotesModule> logger)
            : base(httpClient, logger)
        {
        }

        public Task<CmcQuote> GetQuoteAsync(int tokenId, CancellationToken cancellationToken)
        {
            var uri = string.Format(CmcUriHelper.Quotes.Latest, tokenId);
            return GetAsync<CmcQuote>(uri, cancellationToken);
        }
    }
}