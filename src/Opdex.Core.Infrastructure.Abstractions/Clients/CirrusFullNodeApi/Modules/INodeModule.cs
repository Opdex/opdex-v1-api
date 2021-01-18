using System.Threading;
using System.Threading.Tasks;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;

namespace Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules
{
    public interface INodeModule
    {
        Task<NodeStatusDto> GetNodeStatusAsync(CancellationToken cancellationToken);
    }
}