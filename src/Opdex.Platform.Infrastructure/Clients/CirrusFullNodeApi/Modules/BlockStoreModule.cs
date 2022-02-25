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

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Modules;

public class BlockStoreModule : ApiClientBase, IBlockStoreModule
{
    public BlockStoreModule(HttpClient httpClient, ILogger<BlockStoreModule> logger)
        : base(httpClient, logger, StratisFullNode.SerializerSettings)
    {
    }

    public async Task<BlockReceiptDto> GetBlockAsync(Sha256 blockHash, CancellationToken cancellationToken)
    {
        const bool outputJson = true;
        const bool showTransactionDetails = true;
        var uri = string.Format(CirrusUriHelper.BlockStore.GetBlockByHash, blockHash, outputJson, showTransactionDetails);

        var logDetails = new Dictionary<string, object>
        {
            ["BlockHash"] = blockHash
        };

        using (_logger.BeginScope(logDetails))
        {
            return await GetAsync<BlockReceiptDto>(uri, cancellationToken: cancellationToken);
        }
    }

    public async Task<Sha256> GetBestBlockAsync(CancellationToken cancellationToken)
    {
        return await GetAsync<Sha256>(CirrusUriHelper.Consensus.GetBestBlockHash, cancellationToken: cancellationToken);
    }

    public async Task<Sha256> GetBlockHashAsync(ulong height, CancellationToken cancellationToken)
    {
        var uri = string.Format(CirrusUriHelper.Consensus.GetBlockHash, height);

        var logDetails = new Dictionary<string, object>
        {
            ["BlockHeight"] = height
        };

        using (_logger.BeginScope(logDetails))
        {
            return await GetAsync<Sha256>(uri, cancellationToken: cancellationToken);
        }
    }

    public async Task<AddressesBalancesDto> GetWalletAddressesBalances(IEnumerable<Address> addresses, CancellationToken cancellationToken)
    {
        var uri = string.Format(CirrusUriHelper.BlockStore.GetAddressesBalances, string.Join(',', addresses));
        return await GetAsync<AddressesBalancesDto>(uri, cancellationToken: cancellationToken);
    }
}
