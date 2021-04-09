using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Core.Infrastructure.Http;

namespace Opdex.Core.Infrastructure.Clients.CirrusFullNodeApi.Modules
{
    public class SmartContractsModule : ApiClientBase, ISmartContractsModule
    {
        public SmartContractsModule(HttpClient httpClient, ILogger<SmartContractsModule> logger)
            : base(httpClient, logger)
        {
        }
        
        public Task<ContractCodeDto> GetContractCodeAsync(string address, CancellationToken cancellationToken)
        {
            var uri = string.Format(UriHelper.SmartContracts.GetContractCode, address);
            return GetAsync<ContractCodeDto>(uri, cancellationToken);
        }

        public Task<string> GetContractStorageAsync(string address, string storageKey, string dataType, CancellationToken cancellationToken)
        {
            var uri = string.Format(UriHelper.SmartContracts.GetContractStorageItem, address, storageKey, dataType);
            return GetAsync<string>(uri, cancellationToken);
        }

        public Task<string> GetContractBalanceAsync(string address, CancellationToken cancellationToken)
        {
            var uri = string.Format(UriHelper.SmartContracts.GetContractBalance, address);
            return GetAsync<string>(uri, cancellationToken);
        }

        public Task<TransactionReceiptDto> GetReceiptAsync(string txHash, CancellationToken cancellationToken)
        {
            var uri = string.Format(UriHelper.SmartContracts.GetTransactionReceipt, txHash);
            return GetAsync<TransactionReceiptDto>(uri, cancellationToken);
        }

        public Task<IEnumerable<TransactionReceiptDto>> ReceiptSearchAsync(string contractAddress, string logName, ulong fromBlock, ulong? toBlock, CancellationToken cancellationToken)
        {
            var uri = string.Format(UriHelper.SmartContracts.GetContractReceiptSearch, contractAddress, logName, fromBlock, toBlock);
            
            if (toBlock.GetValueOrDefault() > 0) uri += $"&to={toBlock}";
            
            return GetAsync<IEnumerable<TransactionReceiptDto>>(uri, cancellationToken);
        }

        public Task<LocalCallResponseDto> LocalCallAsync(LocalCallRequestDto request, CancellationToken cancellationToken)
        {
            const string uri = UriHelper.SmartContracts.LocalCall;
            var httpRequest = HttpRequestBuilder.BuildHttpRequestMessage(request, uri, HttpMethod.Post);
            return PostAsync<LocalCallResponseDto>(uri, httpRequest.Content, cancellationToken);
        }

        public async Task<string> CallSmartContractAsync(SmartContractCallRequestDto call, CancellationToken cancellationToken)
        {
            const string uri = UriHelper.SmartContractWallet.Call;
            var httpRequest = HttpRequestBuilder.BuildHttpRequestMessage(call, uri, HttpMethod.Post);
            var response = await PostAsync<SmartContractCallResponseDto>(uri, httpRequest.Content, cancellationToken);

            return response.TransactionId;
        }
    }
}