using System.Threading;
using System.Threading.Tasks;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CoinMarketCapApi.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CoinMarketCapApi.Modules
{
    public interface IQuotesModule
    {
        Task<CmcQuote> GetQuoteAsync(int token, CancellationToken cancellationToken);
    }
}