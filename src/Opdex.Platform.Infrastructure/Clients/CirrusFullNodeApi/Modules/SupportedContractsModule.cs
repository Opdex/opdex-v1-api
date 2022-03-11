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

        // Todo: Temporary static list of supported tokens while the FN Nuget packages are pushed
        // Todo: Uncomment tests in SupportedContractsModuleTests.cs when this is removed
        if (networkType == CirrusNetworkType.Mainnet)
        {
            return new List<InterfluxMappingDto>
            {
                new()
                {
                    NativeNetwork = NativeChainType.Ethereum,
                    NativeChainAddress = "0xC02aaA39b223FE8D0A0e5C4F27eAD9083C756Cc2",
                    Src20Address = "CLgwsVta6KhALRGGv8mtq8ECEAudrhVJT1",
                    TokenName = "WETH",
                    Decimals = 18
                },
                new()
                {
                    NativeNetwork = NativeChainType.Ethereum,
                    NativeChainAddress = "0x2260FAC5E5542a773Aa44fBCfeDf7C193bc2C599",
                    Src20Address = "CPWWnnDJQXcJvP4MCNRhhPWyFcBfv2xnoe",
                    TokenName = "WBTC",
                    Decimals = 8
                },
                new()
                {
                    NativeNetwork = NativeChainType.Ethereum,
                    NativeChainAddress = "0xdAC17F958D2ee523a2206206994597C13D831ec7",
                    Src20Address = "Cf8CJMFADmkLuRNpMfHGk5agJdin3XD8UR",
                    TokenName = "USDT",
                    Decimals = 6
                },
                new()
                {
                    NativeNetwork = NativeChainType.Ethereum,
                    NativeChainAddress = "0x514910771AF9Ca656af840dff83E8264EcF986CA",
                    Src20Address = "CHDyDnoUGvAB9hjLmxXymwWW2WWUmNGRf3",
                    TokenName = "LINK",
                    Decimals = 18
                },
                new()
                {
                    NativeNetwork = NativeChainType.Ethereum,
                    NativeChainAddress = "0x95aD61b0a150d79219dCF64E1E6Cc01f0B64C4cE",
                    Src20Address = "CTqXKirw9qjLWSmbuB9Az53hqGYQ6FCewE",
                    TokenName = "SHIB",
                    Decimals = 18
                }
            };
        }
        else
        {
            return new List<InterfluxMappingDto>
            {
                new()
                {
                    NativeNetwork = NativeChainType.Ethereum,
                    NativeChainAddress = "0xf5dab0f35378ea5fc69149d0f20ba0c16b170a3d",
                    Src20Address = "tQk6t6ithWWoBUQxphDShcYFF6s916mM4R",
                    TokenName = "TSTX",
                    Decimals = 18
                },
                new()
                {
                    NativeNetwork = NativeChainType.Ethereum,
                    NativeChainAddress = "0x2b3b0bd8219ffe0c22ffcdefbc81b7efa5c8d9ba",
                    Src20Address = "tWCCJ3FxmoYuzrE4aUcDLDh9gn51EJ4cvM",
                    TokenName = "TSTY",
                    Decimals = 8
                },
                new()
                {
                    NativeNetwork = NativeChainType.Ethereum,
                    NativeChainAddress = "0x4cb3e0b719a7707c0148e21585d8011213de6708",
                    Src20Address = "tQspjyuEap2vDaNkf9KRHQLdU3h8qq6dnX",
                    TokenName = "TSTZ",
                    Decimals = 6
                }
            };
        }
        // End temporary static list - to be removed when FN has the supported contracts endpoint

        var path = string.Format(CirrusUriHelper.SupportedContracts.List, (int)networkType);
        return await GetAsync<IEnumerable<InterfluxMappingDto>>(path, cancellationToken: cancellationToken);
    }
}
