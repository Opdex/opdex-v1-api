using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Http;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Modules
{
    public class SmartContractsModule : ApiClientBase, ISmartContractsModule
    {
        public SmartContractsModule(HttpClient httpClient, ILogger<SmartContractsModule> logger)
            : base(httpClient, logger)
        {
        }

        public Task<ContractCodeDto> GetContractCodeAsync(string address, CancellationToken cancellationToken)
        {
            var uri = string.Format(CirrusUriHelper.SmartContracts.GetContractCode, address);
            return GetAsync<ContractCodeDto>(uri, cancellationToken);
        }

        public Task<string> GetContractStorageAsync(string address, string storageKey, string dataType, CancellationToken cancellationToken)
        {
            var uri = string.Format(CirrusUriHelper.SmartContracts.GetContractStorageItem, address, storageKey, dataType);
            return GetAsync<string>(uri, cancellationToken);
        }

        public Task<string> GetContractBalanceAsync(string address, CancellationToken cancellationToken)
        {
            var uri = string.Format(CirrusUriHelper.SmartContracts.GetContractBalance, address);
            return GetAsync<string>(uri, cancellationToken);
        }

        public Task<TransactionReceiptDto> GetReceiptAsync(string txHash, CancellationToken cancellationToken)
        {
            var uri = string.Format(CirrusUriHelper.SmartContracts.GetTransactionReceipt, txHash);
            return GetAsync<TransactionReceiptDto>(uri, cancellationToken);
        }

        public Task<IEnumerable<TransactionReceiptDto>> ReceiptSearchAsync(string contractAddress, string logName, ulong fromBlock, ulong? toBlock, CancellationToken cancellationToken)
        {
            var uri = string.Format(CirrusUriHelper.SmartContracts.GetContractReceiptSearch, contractAddress, logName, fromBlock);

            if (toBlock.GetValueOrDefault() > 0) uri += $"&to={toBlock}";

            return GetAsync<IEnumerable<TransactionReceiptDto>>(uri, cancellationToken);
        }

        public Task<LocalCallResponseDto> LocalCallAsync(LocalCallRequestDto request, CancellationToken cancellationToken)
        {
            const string uri = CirrusUriHelper.SmartContracts.LocalCall;
            var httpRequest = HttpRequestBuilder.BuildHttpRequestMessage(request, uri, HttpMethod.Post);
            return PostAsync<LocalCallResponseDto>(uri, httpRequest.Content, cancellationToken);
        }

        public async Task<string> CallSmartContractAsync(SmartContractCallRequestDto call, CancellationToken cancellationToken)
        {
            const string uri = CirrusUriHelper.SmartContractWallet.Call;
            var httpRequest = HttpRequestBuilder.BuildHttpRequestMessage(call, uri, HttpMethod.Post);
            var response = await PostAsync<SmartContractCallResponseDto>(uri, httpRequest.Content, cancellationToken);

            return response.TransactionId;
        }

        public async Task<string> CreateSmartContractAsync(SmartContractCreateRequestDto call, CancellationToken cancellationToken)
        {
            const string uri = CirrusUriHelper.SmartContractWallet.Create;
            var httpRequest = HttpRequestBuilder.BuildHttpRequestMessage(call, uri, HttpMethod.Post);
            var transactionHash = await PostAsync<string>(uri, httpRequest.Content, cancellationToken);

            return transactionHash;
        }

        public Task<ulong> GetWalletAddressCrsBalance(string walletAddress, CancellationToken cancellationToken)
        {
            var uri = string.Format(CirrusUriHelper.SmartContractWallet.AddressBalance, walletAddress);
            return GetAsync<ulong>(uri, cancellationToken);
        }
    }
}
