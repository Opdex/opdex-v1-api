using Opdex.Platform.Common.Models;
using System.Threading;
using System.Threading.Tasks;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;

public interface INodeModule
{
    Task<NodeStatusDto> GetNodeStatusAsync(CancellationToken cancellationToken);

    Task<RawTransactionDto> GetRawTransactionAsync(Sha256 transactionHash, CancellationToken cancellationToken);
}
