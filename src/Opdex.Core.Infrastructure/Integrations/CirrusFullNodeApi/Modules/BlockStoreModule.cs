using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Opdex.Core.Infrastructure.Abstractions.Integrations;
using Opdex.Core.Infrastructure.Abstractions.Integrations.CirrusFullNodeApi.Models;
using Opdex.Core.Infrastructure.Abstractions.Integrations.CirrusFullNodeApi.Modules;

namespace Opdex.BasePlatform.Infrastructure.Integrations.CirrusFullNodeApi.Modules
{
    public class BlockStoreModule : IBlockStoreModule
    {
        private readonly IApiClient _apiClient;
        private readonly ILogger<BlockStoreModule> _logger;
        private const string _apiUrl = "http://localhost:37223/api/BlockStore/";


        public BlockStoreModule(IApiClient apiClient, ILogger<BlockStoreModule> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
            // _apiUrl = $"{cirrusConfiguration1.ApiUrl}api/BlockStore/";
        }

        public async Task<BlockDto> GetBlockAsync(string blockHash, CancellationToken cancellationToken = default)
        {
            var uri = $"{_apiUrl}block?hash={blockHash}&OutputJson=true";

            try
            {
                return await _apiClient.GetAsync<BlockDto>(uri, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting block details", ex);
            }

            // Todo: should be returning a status model
            return default;
        }
    }
}