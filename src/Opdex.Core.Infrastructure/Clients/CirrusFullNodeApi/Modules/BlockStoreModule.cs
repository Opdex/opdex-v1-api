using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Core.Infrastructure.Http;

namespace Opdex.Core.Infrastructure.Clients.CirrusFullNodeApi.Modules
{
    public class BlockStoreModule : ApiClientBase, IBlockStoreModule
    {
        public BlockStoreModule(HttpClient httpClient, ILogger<BlockStoreModule> logger)
            : base(httpClient, logger)
        {
        }

        public Task<BlockReceiptDto> GetBlockAsync(string blockHash, CancellationToken cancellationToken)
        {
            const bool outputJson = true;
            var uri = string.Format(UriHelper.BlockStore.GetBlockByHash, blockHash, outputJson);
            return GetAsync<BlockReceiptDto>(uri, cancellationToken);
        }

        public Task<string> GetBestBlockAsync(CancellationToken cancellationToken)
        {
            return GetAsync<string>(UriHelper.Consensus.GetBestBlockHash, cancellationToken);
        }
    }
}