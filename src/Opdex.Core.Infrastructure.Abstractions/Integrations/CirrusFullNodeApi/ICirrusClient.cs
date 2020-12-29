using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Opdex.Core.Infrastructure.Abstractions.Integrations.CirrusFullNodeApi.Models;

namespace Opdex.Core.Infrastructure.Abstractions.Integrations.CirrusFullNodeApi
{
    public interface ICirrusClient
    {
        Task<ReceiptDto> GetReceiptAsync(string txHash, CancellationToken cancellationToken = default);
        Task<IEnumerable<ReceiptDto>> ReceiptSearchAsync(string contractAddress, string eventName, ulong fromBlock, 
            ulong toBlock, CancellationToken cancellationToken = default);
        Task<LocalCallResponseDto> LocalCallAsync(LocalCallRequestDto request, CancellationToken cancellationToken = default);
        Task<BlockDto> GetBlockAsync(string blockHash, CancellationToken cancellationToken = default);
        Task<TokenDto> GetTokenDetails(string address);
        Task<ulong> GetSrcAllowance(string tokenAddress, string owner, string spender, CancellationToken cancellationToken);
        Task<ulong> GetSrcBalance(string tokenAddress, string owner, CancellationToken cancellationToken);
    }
}