using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;

namespace Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules
{
    public interface ISmartContractsModule
    {
        Task<ContractCodeDto> GetContractCodeAsync(string address, CancellationToken cancellationToken);
        Task<string> GetContractStorageAsync(string address, string storageKey, string dataType, CancellationToken cancellationToken);
        Task<string> GetContractBalanceAsync(string address, CancellationToken cancellationToken);
        Task<ReceiptDto> GetReceiptAsync(string txHash, CancellationToken cancellationToken);
        Task<IEnumerable<ReceiptDto>> ReceiptSearchAsync(string contractAddress, string eventName, ulong fromBlock, ulong toBlock, CancellationToken cancellationToken);
        Task<LocalCallResponseDto> LocalCallAsync(LocalCallRequestDto request, CancellationToken cancellationToken);
    }
}