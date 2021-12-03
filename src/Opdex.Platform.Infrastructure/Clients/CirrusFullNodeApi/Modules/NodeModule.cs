using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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

    public Task<NodeStatusDto> GetNodeStatusAsync(CancellationToken cancellationToken)
    {
        return GetAsync<NodeStatusDto>(CirrusUriHelper.Node.Status, cancellationToken);
    }
}