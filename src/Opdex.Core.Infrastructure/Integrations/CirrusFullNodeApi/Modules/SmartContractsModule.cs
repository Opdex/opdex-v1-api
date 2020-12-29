using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Opdex.Core.Infrastructure.Abstractions.Integrations;
using Opdex.Core.Infrastructure.Abstractions.Integrations.CirrusFullNodeApi.Models;
using Opdex.Core.Infrastructure.Abstractions.Integrations.CirrusFullNodeApi.Modules;

namespace Opdex.BasePlatform.Infrastructure.Integrations.CirrusFullNodeApi.Modules
{
    public class SmartContractsModule : ISmartContractsModule
    {
        private readonly IApiClient _apiClient;
        private readonly ILogger<SmartContractsModule> _logger;
        private const string _apiUrl = "http://localhost:37223/api/SmartContracts/";

        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.None,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public SmartContractsModule(IApiClient apiClient, ILogger<SmartContractsModule> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }
        
        public async Task<ContractCodeDto> GetContractCodeAsync(string address, CancellationToken cancellationToken = default)
        {
            var uri = $"{_apiUrl}code?address={address}";

            try
            {
                return await _apiClient.GetAsync<ContractCodeDto>(uri, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting contract code");
            }

            // Todo: should be returning a status model
            return default;
        }

        public async Task<string> GetContractStorageAsync(string address, string storageKey, string dataType, CancellationToken cancellationToken = default)
        {
            var uri = $"{_apiUrl}storage?ContractAddress={address}&StorageKey={storageKey}&DataType={dataType}";

            try
            {
                return await _apiClient.GetAsync<string>(uri, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting contract code");
            }

            // Todo: should be returning a status model
            return null;
        }

        public async Task<string> GetContractBalanceAsync(string address, CancellationToken cancellationToken = default)
        {
            var uri = $"{_apiUrl}balance?address={address}";

            try
            {
                return await _apiClient.GetAsync<string>(uri, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting contract code");
            }

            // Todo: should be returning a status model
            return string.Empty;
        }

        public async Task<ReceiptDto> GetReceiptAsync(string txHash, CancellationToken cancellationToken = default)
        {
            var uri = $"{_apiUrl}receipt?txHash={txHash}";

            try
            {
                return await _apiClient.GetAsync<ReceiptDto>(uri, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting receipt search");
            }

            // Todo: should be returning a status model
            return default;
        }

        public async Task<IEnumerable<ReceiptDto>> ReceiptSearchAsync(string contractAddress, string eventName, ulong fromBlock, ulong toBlock, CancellationToken cancellationToken = default)
        {
            //var uri = $"{_apiUrl}receipt-search?contractAddress={contractAddress}&eventName={eventName}";

            var uri = $"{_apiUrl}receipt-search?contractAddress={contractAddress}&eventName={eventName}&fromBlock={fromBlock}&toBlock={toBlock}";

            try
            {
                return await _apiClient.GetAsync<IEnumerable<ReceiptDto>>(uri, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting receipt search");
            }

            // Todo: should be returning a status model
            return default;
        }

        public async Task<LocalCallResponseDto> LocalCallAsync(LocalCallRequestDto request, CancellationToken cancellationToken = default)
        {
            var uri = $"{_apiUrl}local-call";

            var json = JsonConvert.SerializeObject(request, JsonSerializerSettings);
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            try
            {
                return await _apiClient.PostAsync<LocalCallResponseDto>(uri, httpRequest.Content, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting receipt search");
            }

            // Todo: should be returning a status model
            return default;
        }
    }
}