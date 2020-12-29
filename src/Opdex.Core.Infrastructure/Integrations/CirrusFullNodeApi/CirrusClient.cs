using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Opdex.BasePlatform.Common.Extensions;
using Opdex.Core.Infrastructure.Abstractions.Integrations.CirrusFullNodeApi;
using Opdex.Core.Infrastructure.Abstractions.Integrations.CirrusFullNodeApi.Models;
using Opdex.Core.Infrastructure.Abstractions.Integrations.CirrusFullNodeApi.Modules;

namespace Opdex.BasePlatform.Infrastructure.Integrations.CirrusFullNodeApi
{
    public class CirrusClient : ICirrusClient
    {

        private readonly ISmartContractsModule _smartContractsModule;
        private readonly IBlockStoreModule _blockStoreModule;
        private readonly ILogger<CirrusClient> _logger;

        public CirrusClient(ISmartContractsModule smartContractsModule,
            IBlockStoreModule blockStoreModule, ILogger<CirrusClient>  logger)
        {
            _smartContractsModule = smartContractsModule;
            _blockStoreModule = blockStoreModule;
            _logger = logger;
        }

        public async Task<ReceiptDto> GetReceiptAsync(string txHash, CancellationToken cancellationToken = default)
        {
            var result = await _smartContractsModule.GetReceiptAsync(txHash, cancellationToken);

            return result;
        }

        public async Task<IEnumerable<ReceiptDto>> ReceiptSearchAsync(string contractAddress, string eventName, ulong fromBlock, ulong toBlock, CancellationToken cancellationToken = default)
        {
            var result = await _smartContractsModule.ReceiptSearchAsync(contractAddress, eventName, fromBlock, toBlock, cancellationToken);

            return result;
        }

        public async Task<LocalCallResponseDto> LocalCallAsync(LocalCallRequestDto request, CancellationToken cancellationToken = default)
        {
            var result = await _smartContractsModule.LocalCallAsync(request, cancellationToken);

            return result;
        }

        public async Task<BlockDto> GetBlockAsync(string blockHash, CancellationToken cancellationToken = default)
        {
            var result = await _blockStoreModule.GetBlockAsync(blockHash, cancellationToken);

            return result;
        }

        public async Task<TokenDto> GetTokenDetails(string address)
        {
            //Todo: Check for validity here
            var name = await _smartContractsModule.GetContractStorageAsync(address, "Name", "string");
            if (!name.HasValue())
            {
                return null;
            }

            var ticker = await _smartContractsModule.GetContractStorageAsync(address, "Symbol", "string");
            var decimalString = await _smartContractsModule.GetContractStorageAsync(address, "Decimals", "uint");

            var parseDecimalsSuccess = short.TryParse(decimalString, out var decimals);

            return new TokenDto
            {
                Address = address,
                Name = name,
                Ticker = ticker,
                Decimals = parseDecimalsSuccess ? decimals : (short)8
            };
        }

        public async Task<ulong> GetSrcAllowance(string tokenAddress, string owner, string spender, CancellationToken cancellationToken)
        {
            var parameters = new[] {$"9#{owner}", $"9#{spender}"};
            var allowanceCall = new LocalCallRequestDto(tokenAddress, spender, "Allowance", parameters);

            var allowance = await LocalCallAsync(allowanceCall, cancellationToken);
            ulong.TryParse(allowance.Return.ToString(), out var orderBookAllowance);

            return orderBookAllowance;
        }

        public async Task<ulong> GetSrcBalance(string tokenAddress, string owner, CancellationToken cancellationToken)
        {
            var parameters = new[] {$"9#{owner}"};
            var request = new LocalCallRequestDto(tokenAddress, owner, "GetBalance", parameters);

            var balance = await LocalCallAsync(request, cancellationToken);
            ulong.TryParse(balance.Return.ToString(), out var srcBalance);

            return srcBalance;
        }
    }
}