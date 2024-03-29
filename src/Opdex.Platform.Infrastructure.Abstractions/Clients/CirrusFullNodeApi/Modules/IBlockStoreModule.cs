using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;

public interface IBlockStoreModule
{
    Task<BlockReceiptDto> GetBlockAsync(Sha256 blockHash, CancellationToken cancellationToken);
    Task<Sha256> GetBestBlockAsync(CancellationToken cancellationToken);
    Task<Sha256> GetBlockHashAsync(ulong height, CancellationToken cancellationToken);
    Task<AddressesBalancesDto> GetWalletAddressesBalances(IEnumerable<Address> addresses, CancellationToken cancellationToken);
}