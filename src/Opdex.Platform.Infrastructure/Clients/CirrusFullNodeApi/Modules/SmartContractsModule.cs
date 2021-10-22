using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models.Transactions;
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

        public Task<ContractCodeDto> GetContractCodeAsync(Address address, CancellationToken cancellationToken)
        {
            var uri = string.Format(CirrusUriHelper.SmartContracts.GetContractCode, address);
            return GetAsync<ContractCodeDto>(uri, cancellationToken);
        }

        public async Task<string> GetContractStorageAsync(Address address, string storageKey, SmartContractParameterType dataType,
                                                    ulong blockHeight, CancellationToken cancellationToken)
        {
            var uri = string.Format(CirrusUriHelper.SmartContracts.GetContractStorageItem, address, storageKey, ((uint)dataType).ToString());

            if (blockHeight > 0) uri += $"&BlockHeight={blockHeight}";

            return await GetAsync<string>(uri, cancellationToken);
        }

        public Task<string> GetContractBalanceAsync(Address address, CancellationToken cancellationToken)
        {
            var uri = string.Format(CirrusUriHelper.SmartContracts.GetContractBalance, address);
            return GetAsync<string>(uri, cancellationToken);
        }

        public Task<TransactionReceiptDto> GetReceiptAsync(Sha256 txHash, CancellationToken cancellationToken)
        {
            var uri = string.Format(CirrusUriHelper.SmartContracts.GetTransactionReceipt, txHash);
            return GetAsync<TransactionReceiptDto>(uri, cancellationToken);
        }

        public Task<IEnumerable<TransactionReceiptDto>> ReceiptSearchAsync(Address contractAddress, string logName, ulong fromBlock,
                                                                           ulong? toBlock, CancellationToken cancellationToken)
        {
            var uri = string.Format(CirrusUriHelper.SmartContracts.GetContractReceiptSearch, contractAddress, logName, fromBlock);

            if (toBlock.GetValueOrDefault() > 0) uri += $"&to={toBlock}";

            return GetAsync<IEnumerable<TransactionReceiptDto>>(uri, cancellationToken);
        }

        public async Task<LocalCallResponseDto> LocalCallAsync(LocalCallRequestDto request, CancellationToken cancellationToken)
        {
            const string uri = CirrusUriHelper.SmartContracts.LocalCall;
            var httpRequest = HttpRequestBuilder.BuildHttpRequestMessage(request, uri, HttpMethod.Post);

            var logDetails = new Dictionary<string, object>
            {
                ["Contract"] = request.ContractAddress,
                ["Method"] = request.MethodName,
                ["Sender"] = request.Sender,
                ["Amount"] = request.Amount,
                ["Parameters"] = request.Parameters
            };

            if (request.BlockHeight.HasValue) logDetails.Add("BlockHeight", request.BlockHeight.Value);

            using (_logger.BeginScope(logDetails))
            {
                return await PostAsync<LocalCallResponseDto>(uri, httpRequest.Content, cancellationToken);
            }
        }

        public async Task<Sha256> CallSmartContractAsync(SmartContractCallRequestDto call, CancellationToken cancellationToken)
        {
            const string uri = CirrusUriHelper.SmartContractWallet.Call;
            var httpRequest = HttpRequestBuilder.BuildHttpRequestMessage(call, uri, HttpMethod.Post);

            var logDetails = new Dictionary<string, object>
            {
                ["Contract"] = call.ContractAddress,
                ["Method"] = call.MethodName,
                ["Sender"] = call.Sender,
                ["Amount"] = call.Amount,
                ["Parameters"] = call.Parameters,
                ["WalletName"] = call.WalletName
            };

            using (_logger.BeginScope(logDetails))
            {
                var response = await PostAsync<SmartContractCallResponseDto>(uri, httpRequest.Content, cancellationToken);
                return response.TransactionId;
            }
        }

        public async Task<Sha256> CreateSmartContractAsync(SmartContractCreateRequestDto call, CancellationToken cancellationToken)
        {
            const string uri = CirrusUriHelper.SmartContractWallet.Create;
            var httpRequest = HttpRequestBuilder.BuildHttpRequestMessage(call, uri, HttpMethod.Post);

            var logDetails = new Dictionary<string, object>
            {
                ["ContractCode"] = call.ContractCode,
                ["Sender"] = call.Sender,
                ["Amount"] = call.Amount,
                ["Parameters"] = call.Parameters,
                ["WalletName"] = call.WalletName
            };

            using (_logger.BeginScope(logDetails))
            {
                var transactionHash = await PostAsync<Sha256>(uri, httpRequest.Content, cancellationToken);
                return transactionHash;
            }
        }
    }
}
