using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Core.Infrastructure.Http;

namespace Opdex.Core.Infrastructure.Clients.CirrusFullNodeApi.Modules
{
    public class NodeModule : ApiClientBase, INodeModule
    {
        public NodeModule(HttpClient httpClient, ILogger<NodeModule> logger)
            : base(httpClient, logger)
        {
        }

        public Task<NodeStatusDto> GetNodeStatusAsync(CancellationToken cancellationToken)
        {
            return GetAsync<NodeStatusDto>(UriHelper.Node.Status, cancellationToken);
        }
    }
}