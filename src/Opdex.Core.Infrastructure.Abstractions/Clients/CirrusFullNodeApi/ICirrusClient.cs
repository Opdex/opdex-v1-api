using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;

namespace Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi
{
    public interface ICirrusClient
    {
        // Task<ReceiptDto> GetReceiptAsync(string txHash, CancellationToken cancellationToken);
        // Task<IEnumerable<ReceiptDto>> ReceiptSearchAsync(string contractAddress, string eventName, ulong fromBlock, 
        //     ulong toBlock, CancellationToken cancellationToken);
        Task<LocalCallResponseDto> LocalCallAsync(LocalCallRequestDto request, CancellationToken cancellationToken);
        Task<BlockDto> GetBlockAsync(string blockHash, CancellationToken cancellationToken);
        Task<TokenDto> GetTokenDetails(string address, CancellationToken cancellationToken);
        Task<ulong> GetSrcAllowance(string tokenAddress, string owner, string spender, CancellationToken cancellationToken);
        Task<ulong> GetSrcBalance(string tokenAddress, string owner, CancellationToken cancellationToken);
    }
}