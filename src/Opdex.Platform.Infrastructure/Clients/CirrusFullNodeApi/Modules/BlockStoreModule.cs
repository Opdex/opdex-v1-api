using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Http;
using System.Collections.Generic;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Modules
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
            var uri = string.Format(CirrusUriHelper.BlockStore.GetBlockByHash, blockHash, outputJson);

            var logDetails = new Dictionary<string, object>
            {
                ["BlockHash"] = blockHash
            };

            using (_logger.BeginScope(logDetails))
            {
                return GetAsync<BlockReceiptDto>(uri, cancellationToken);
            }
        }

        public Task<string> GetBestBlockAsync(CancellationToken cancellationToken)
        {
            return GetAsync<string>(CirrusUriHelper.Consensus.GetBestBlockHash, cancellationToken);
        }

        public Task<string> GetBlockHashAsync(ulong height, CancellationToken cancellationToken)
        {
            var uri = string.Format(CirrusUriHelper.Consensus.GetBlockHash, height);

            var logDetails = new Dictionary<string, object>
            {
                ["BlockHeight"] = height
            };

            using (_logger.BeginScope(logDetails))
            {
                return GetAsync<string>(uri, cancellationToken);
            }
        }

        public async Task<AddressesBalancesDto> GetWalletAddressesBalances(IEnumerable<Address> addresses, CancellationToken cancellationToken)
        {
            var uri = string.Format(CirrusUriHelper.BlockStore.GetAddressesBalances, string.Join(',', addresses));
            return await GetAsync<AddressesBalancesDto>(uri, cancellationToken);
        }
    }
}
