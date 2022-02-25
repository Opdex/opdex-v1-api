using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Serialization;
using Opdex.Platform.Infrastructure.Http;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Modules;

public class NodeModule : ApiClientBase, INodeModule
{
    public NodeModule(HttpClient httpClient, ILogger<NodeModule> logger)
        : base(httpClient, logger, StratisFullNode.SerializerSettings)
    {
    }

    public async Task<NodeStatusDto> GetNodeStatusAsync(CancellationToken cancellationToken)
    {
        return await GetAsync<NodeStatusDto>(CirrusUriHelper.Node.Status, cancellationToken: cancellationToken);
    }

    public async Task<RawTransactionDto> GetRawTransactionAsync(Sha256 transactionHash, CancellationToken cancellationToken)
    {
        return await GetAsync<RawTransactionDto>(string.Format(CirrusUriHelper.Node.GetRawTransaction, transactionHash, true), cancellationToken: cancellationToken);
    }
}
