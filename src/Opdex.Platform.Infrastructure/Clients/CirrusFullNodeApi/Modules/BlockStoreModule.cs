using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Http;
using System.Collections.Generic;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Serialization;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Modules
{
    public class BlockStoreModule : ApiClientBase, IBlockStoreModule
    {
        public BlockStoreModule(HttpClient httpClient, ILogger<BlockStoreModule> logger)
            : base(httpClient, logger, StratisFullNode.SerializerSettings)
        {
        }

        public Task<BlockReceiptDto> GetBlockAsync(Sha256 blockHash, CancellationToken cancellationToken)
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

        public Task<Sha256> GetBestBlockAsync(CancellationToken cancellationToken)
        {
            return GetAsync<Sha256>(CirrusUriHelper.Consensus.GetBestBlockHash, cancellationToken);
        }

        public Task<Sha256> GetBlockHashAsync(ulong height, CancellationToken cancellationToken)
        {
            var uri = string.Format(CirrusUriHelper.Consensus.GetBlockHash, height);

            var logDetails = new Dictionary<string, object>
            {
                ["BlockHeight"] = height
            };

            using (_logger.BeginScope(logDetails))
            {
                return GetAsync<Sha256>(uri, cancellationToken);
            }
        }

        public async Task<AddressesBalancesDto> GetWalletAddressesBalances(IEnumerable<Address> addresses, CancellationToken cancellationToken)
        {
            var uri = string.Format(CirrusUriHelper.BlockStore.GetAddressesBalances, string.Join(',', addresses));
            return await GetAsync<AddressesBalancesDto>(uri, cancellationToken);
        }
    }
}
