using Microsoft.Extensions.Logging;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Serialization;
using Opdex.Platform.Infrastructure.Http;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Modules
{
    public class WalletModule : ApiClientBase, IWalletModule
    {
        public WalletModule(HttpClient httpClient, ILogger<WalletModule> logger) : base(httpClient, logger, StratisFullNode.SerializerSettings)
        {
        }

        /// <inheritdoc />
        public async Task<bool> VerifyMessage(VerifyMessageRequestDto request, CancellationToken cancellationToken)
        {
            var httpRequest = HttpRequestBuilder.BuildHttpRequestMessage(request, CirrusUriHelper.Wallet.VerifyMessage, HttpMethod.Post, _serializerSettings);

            var logDetails = new Dictionary<string, object>
            {
                ["Message"] = request.Message,
                ["Signer"] = request.ExternalAddress,
                ["Signature"] = request.Signature
            };

            using (_logger.BeginScope(logDetails))
            {
                return await PostAsync<bool>(CirrusUriHelper.Wallet.VerifyMessage, httpRequest.Content, cancellationToken);
            }
        }
    }
}