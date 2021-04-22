using System.Threading;
using System.Threading.Tasks;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules
{
    public interface IBlockStoreModule
    {
        Task<BlockReceiptDto> GetBlockAsync(string blockHash, CancellationToken cancellationToken);
        Task<string> GetBestBlockAsync(CancellationToken cancellationToken);
    }
}