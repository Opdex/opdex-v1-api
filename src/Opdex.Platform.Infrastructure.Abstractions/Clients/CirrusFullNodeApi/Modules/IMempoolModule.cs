using Opdex.Platform.Common.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules
{
    public interface IMempoolModule
    {
        /// <summary>Retrieves a collection of transactions that exist within the mempool.</summary>
        /// <remarks>The mempool stores transactions which have not yet been confirmed by the network.</remarks>
        /// <returns>Collection of transaction hashes.</returns>
        Task<IEnumerable<Sha256>> GetRawMempoolAsync(CancellationToken cancellationToken);
    }
}
