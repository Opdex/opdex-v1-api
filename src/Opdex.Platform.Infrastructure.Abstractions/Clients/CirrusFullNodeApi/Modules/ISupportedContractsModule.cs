using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;

public interface ISupportedContractsModule
{
    Task<IEnumerable<InterfluxMappingDto>> GetList(CancellationToken cancellationToken = default);
}
