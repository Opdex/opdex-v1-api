using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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

        public Task<ReceiptDto> GetReceiptAsync(string txHash, CancellationToken cancellationToken)
        {
            var uri = string.Format(UriHelper.SmartContracts.GetTransactionReceipt, txHash);
            return GetAsync<ReceiptDto>(uri, cancellationToken);
        }

        public Task<IEnumerable<ReceiptDto>> ReceiptSearchAsync(string contractAddress, string eventName, ulong fromBlock, ulong toBlock, CancellationToken cancellationToken)
        {
            var uri = string.Format(UriHelper.SmartContracts.GetContractReceiptSearch, contractAddress, eventName, fromBlock, toBlock);
            return GetAsync<IEnumerable<ReceiptDto>>(uri, cancellationToken);
        }

        public async Task<LocalCallResponseDto> LocalCallAsync(LocalCallRequestDto request, CancellationToken cancellationToken)
        {
            const string uri = UriHelper.SmartContracts.LocalCall;

            // Todo: Clean this up with general HttpContent builders
            var json = JsonConvert.SerializeObject(request, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            return await PostAsync<LocalCallResponseDto>(uri, httpRequest.Content, cancellationToken);
        }
    }
}