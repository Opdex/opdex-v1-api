using Opdex.Platform.Common.Enums;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models.Transactions;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;

public interface ISmartContractsModule
{
    Task<ContractCodeDto> GetContractCodeAsync(Address address, CancellationToken cancellationToken);
    Task<string> GetContractStorageAsync(Address address, string storageKey, SmartContractParameterType dataType, ulong blockHeight, CancellationToken cancellationToken);
    Task<string> GetContractBalanceAsync(Address address, CancellationToken cancellationToken);
    Task<TransactionReceiptDto> GetReceiptAsync(Sha256 txHash, CancellationToken cancellationToken);
    Task<IEnumerable<TransactionReceiptDto>> ReceiptSearchAsync(Address contractAddress, string logName, ulong fromBlock, ulong? toBlock, CancellationToken cancellationToken);
    Task<LocalCallResponseDto> LocalCallAsync(LocalCallRequestDto request, CancellationToken cancellationToken);
    Task<Sha256> CallSmartContractAsync(SmartContractCallRequestDto call, CancellationToken cancellationToken);
    Task<Sha256> CreateSmartContractAsync(SmartContractCreateRequestDto call, CancellationToken cancellationToken);
}