using System.Threading;
using System.Threading.Tasks;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;

namespace Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules
{
    public interface IBlockStoreModule
    {
        Task<BlockReceiptDto> GetBlockAsync(string blockHash, CancellationToken cancellationToken);
        Task<string> GetBestBlockAsync(CancellationToken cancellationToken);
    }
}