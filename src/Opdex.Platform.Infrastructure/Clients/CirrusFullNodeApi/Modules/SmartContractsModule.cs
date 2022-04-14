using Microsoft.Extensions.Logging;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Serialization;
using Opdex.Platform.Infrastructure.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Modules;

public class SmartContractsModule : ApiClientBase, ISmartContractsModule
{
    public SmartContractsModule(HttpClient httpClient, ILogger<SmartContractsModule> logger)
        : base(httpClient, logger, StratisFullNode.SerializerSettings)
    {
    }

    public async Task<ContractCodeDto> GetContractCodeAsync(Address address, CancellationToken cancellationToken)
    {
        var uri = string.Format(CirrusUriHelper.SmartContracts.GetContractCode, address);
        return await GetAsync<ContractCodeDto>(uri, cancellationToken: cancellationToken);
    }

    public async Task<string> GetContractStorageAsync(Address address, string storageKey, SmartContractParameterType dataType,
                                                      ulong blockHeight, CancellationToken cancellationToken)
    {
        var uri = string.Format(CirrusUriHelper.SmartContracts.GetContractStorageItem, address, storageKey, ((uint)dataType).ToString());

        if (blockHeight > 0) uri += $"&BlockHeight={blockHeight}";

        return await GetAsync<string>(uri, cancellationToken: cancellationToken);
    }

    public async Task<string> GetContractBalanceAsync(Address address, CancellationToken cancellationToken)
    {
        var uri = string.Format(CirrusUriHelper.SmartContracts.GetContractBalance, address);
        return await GetAsync<string>(uri, cancellationToken: cancellationToken);
    }

    public async Task<TransactionReceiptDto> GetReceiptAsync(Sha256 txHash, CancellationToken cancellationToken)
    {
        var uri = string.Format(CirrusUriHelper.SmartContracts.GetTransactionReceipt, txHash);
        return await GetAsync<TransactionReceiptDto>(uri, cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<TransactionReceiptDto>> ReceiptSearchAsync(Address contractAddress, string logName, ulong fromBlock,
                                                                       ulong? toBlock, CancellationToken cancellationToken)
    {
        var uri = string.Format(CirrusUriHelper.SmartContracts.GetContractReceiptSearch, contractAddress, logName, fromBlock);

        if (toBlock.GetValueOrDefault() > 0) uri += $"&to={toBlock}";

        return await GetAsync<IEnumerable<TransactionReceiptDto>>(uri, cancellationToken: cancellationToken);
    }

    public async Task<LocalCallResponseDto> LocalCallAsync(LocalCallRequestDto request, CancellationToken cancellationToken)
    {
        const string uri = CirrusUriHelper.SmartContracts.LocalCall;
        var httpRequest = HttpRequestBuilder.BuildHttpRequestMessage(request, uri, HttpMethod.Post, _serializerSettings);

        var logDetails = new Dictionary<string, object>
        {
            ["Contract"] = request.ContractAddress,
            ["Method"] = request.MethodName,
            ["Sender"] = request.Sender,
            ["Amount"] = request.Amount,
            ["Parameters"] = request.Parameters.Select(parameter => parameter.Serialize())
        };

        if (request.BlockHeight.HasValue) logDetails.Add("BlockHeight", request.BlockHeight.Value);

        using (_logger.BeginScope(logDetails))
        {
            LocalCallResponseDto response = null;
            for (var retryCount = 0; retryCount < 3; retryCount++)
            {
                response = await PostAsync<LocalCallResponseDto>(uri, httpRequest.Content, cancellationToken: cancellationToken);
                if (response.HasError && response.ErrorMessage.Value == "No code at this address.")
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(500 + (500 * retryCount)), cancellationToken);
                    continue;
                }

                break;
            }

            return response;
        }
    }

    public async Task<Sha256> CallSmartContractAsync(SmartContractCallRequestDto call, CancellationToken cancellationToken)
    {
        const string uri = CirrusUriHelper.SmartContractWallet.Call;
        var httpRequest = HttpRequestBuilder.BuildHttpRequestMessage(call, uri, HttpMethod.Post, _serializerSettings);

        var logDetails = new Dictionary<string, object>
        {
            ["Contract"] = call.ContractAddress,
            ["Method"] = call.MethodName,
            ["Sender"] = call.Sender,
            ["Amount"] = call.Amount,
            ["Parameters"] = call.Parameters.Select(parameter => parameter.Serialize()),
            ["WalletName"] = call.WalletName
        };

        using (_logger.BeginScope(logDetails))
        {
            var response = await PostAsync<SmartContractCallResponseDto>(uri, httpRequest.Content, cancellationToken: cancellationToken);
            return response.TransactionId;
        }
    }

    public async Task<Sha256> CreateSmartContractAsync(SmartContractCreateRequestDto call, CancellationToken cancellationToken)
    {
        const string uri = CirrusUriHelper.SmartContractWallet.Create;
        var httpRequest = HttpRequestBuilder.BuildHttpRequestMessage(call, uri, HttpMethod.Post, _serializerSettings);

        var logDetails = new Dictionary<string, object>
        {
            ["ContractCode"] = call.ContractCode,
            ["Sender"] = call.Sender,
            ["Amount"] = call.Amount,
            ["Parameters"] = call.Parameters.Select(parameter => parameter.Serialize()),
            ["WalletName"] = call.WalletName
        };

        using (_logger.BeginScope(logDetails))
        {
            var transactionHash = await PostAsync<Sha256>(uri, httpRequest.Content, cancellationToken: cancellationToken);
            return transactionHash;
        }
    }
}
