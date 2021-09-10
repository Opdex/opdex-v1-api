using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules
{
    public interface ISmartContractsModule
    {
        Task<ContractCodeDto> GetContractCodeAsync(Address address, CancellationToken cancellationToken);
        Task<string> GetContractStorageAsync(Address address, string storageKey, string dataType, CancellationToken cancellationToken);
        Task<string> GetContractBalanceAsync(Address address, CancellationToken cancellationToken);
        Task<TransactionReceiptDto> GetReceiptAsync(string txHash, CancellationToken cancellationToken);
        Task<IEnumerable<TransactionReceiptDto>> ReceiptSearchAsync(Address contractAddress, string logName, ulong fromBlock, ulong? toBlock, CancellationToken cancellationToken);
        Task<LocalCallResponseDto> LocalCallAsync(LocalCallRequestDto request, CancellationToken cancellationToken);
        Task<string> CallSmartContractAsync(SmartContractCallRequestDto call, CancellationToken cancellationToken);
        Task<string> CreateSmartContractAsync(SmartContractCreateRequestDto call, CancellationToken cancellationToken);
        Task<ulong> GetWalletAddressCrsBalance(string walletAddress, CancellationToken cancellationToken);
    }
}
