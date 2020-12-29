using System.Threading;
using System.Threading.Tasks;
using Opdex.Core.Infrastructure.Abstractions.Integrations.CirrusFullNodeApi.Models;

namespace Opdex.Core.Infrastructure.Abstractions.Integrations.CirrusFullNodeApi.Modules
{
    public interface IBlockStoreModule
    {
        Task<BlockDto> GetBlockAsync(string blockHash, CancellationToken cancellationToken = default);
    }
}