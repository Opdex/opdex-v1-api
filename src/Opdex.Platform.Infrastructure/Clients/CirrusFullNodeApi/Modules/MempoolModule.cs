using Microsoft.Extensions.Logging;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Http;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Modules
{
    public class MempoolModule : ApiClientBase, IMempoolModule
    {
        public MempoolModule(HttpClient httpClient, ILogger logger) : base(httpClient, logger)
        {
        }

        /// <inheritdoc />
        public async Task<IEnumerable<string>> GetRawMempoolAsync(CancellationToken cancellationToken)
        {
            return await GetAsync<IEnumerable<string>>(CirrusUriHelper.Mempool.GetRawMempool, cancellationToken);
        }
    }
}
