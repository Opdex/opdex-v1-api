using Microsoft.Extensions.Logging;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Serialization;
using Opdex.Platform.Infrastructure.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Modules;

public class SupportedContractsModule : ApiClientBase, ISupportedContractsModule
{
    private readonly OpdexConfiguration _opdexConfig;

    public SupportedContractsModule(HttpClient httpClient, ILogger<SupportedContractsModule> logger,
                                    OpdexConfiguration opdexConfig)
        : base(httpClient, logger, StratisFullNode.SerializerSettings)
    {
        _opdexConfig = opdexConfig ?? throw new ArgumentNullException(nameof(opdexConfig));
    }

    public async Task<IEnumerable<InterfluxMappingDto>> GetList(CancellationToken cancellationToken = default)
    {
        if (_opdexConfig.Network == NetworkType.DEVNET) return Array.Empty<InterfluxMappingDto>();

        var networkType = _opdexConfig.Network switch
        {
            NetworkType.MAINNET => CirrusNetworkType.Mainnet,
            NetworkType.TESTNET => CirrusNetworkType.Testnet,
            NetworkType.DEVNET or _ => throw new NotSupportedException()
        };

        var path = string.Format(CirrusUriHelper.SupportedContracts.List, (int)networkType);
        return await GetAsync<IEnumerable<InterfluxMappingDto>>(path, cancellationToken: cancellationToken);
    }
}
