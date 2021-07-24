using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules
{
    public interface ISmartContractsModule
    {
        Task<ContractCodeDto> GetContractCodeAsync(string address, CancellationToken cancellationToken);
        Task<string> GetContractStorageAsync(string address, string storageKey, string dataType, CancellationToken cancellationToken);
        Task<string> GetContractBalanceAsync(string address, CancellationToken cancellationToken);
        Task<TransactionReceiptDto> GetReceiptAsync(string txHash, CancellationToken cancellationToken);
        Task<IEnumerable<TransactionReceiptDto>> ReceiptSearchAsync(string contractAddress, string logName, ulong fromBlock, ulong? toBlock, CancellationToken cancellationToken);
        Task<LocalCallResponseDto> LocalCallAsync(LocalCallRequestDto request, CancellationToken cancellationToken);
        Task<string> CallSmartContractAsync(SmartContractCallRequestDto call, CancellationToken cancellationToken);
        Task<string> CreateSmartContractAsync(SmartContractCreateRequestDto call, CancellationToken cancellationToken);
        Task<ulong> GetDevnetWalletCrsBalance(string walletAddress, CancellationToken cancellationToken);
    }
}
