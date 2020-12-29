using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Opdex.Core.Infrastructure.Abstractions.Integrations.CirrusFullNodeApi.Models;

namespace Opdex.Core.Infrastructure.Abstractions.Integrations.CirrusFullNodeApi.Modules
{
    public interface ISmartContractsModule
    {
        Task<ContractCodeDto> GetContractCodeAsync(string address, CancellationToken cancellationToken = default);
        Task<string> GetContractStorageAsync(string address, string storageKey, string dataType, CancellationToken cancellationToken = default);
        Task<string> GetContractBalanceAsync(string address, CancellationToken cancellationToken = default);
        Task<ReceiptDto> GetReceiptAsync(string txHash, CancellationToken cancellationToken = default);
        Task<IEnumerable<ReceiptDto>> ReceiptSearchAsync(string contractAddress, string eventName, ulong fromBlock, ulong toBlock, CancellationToken cancellationToken = default);
        Task<LocalCallResponseDto> LocalCallAsync(LocalCallRequestDto request, CancellationToken cancellationToken = default);
    }
}