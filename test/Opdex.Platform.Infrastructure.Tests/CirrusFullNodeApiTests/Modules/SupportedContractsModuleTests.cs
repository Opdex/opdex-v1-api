using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moq.Contrib.HttpClient;
using Newtonsoft.Json;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Serialization;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.CirrusFullNodeApiTests.Modules;

public class SupportedContractsModuleTests
{
    private const string BaseAddress = "https://cirrus.node";

    private Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private SupportedContractsModule _supportedContractsModule;

    [Fact]
    public async Task GetList_Devnet_DoNotCallNode()
    {
        // Arrange
        SetupClient(NetworkType.DEVNET);

        // Act
        await _supportedContractsModule.GetList();

        // Assert
        _httpMessageHandlerMock.VerifyAnyRequest(Times.Never());
    }

    [Fact]
    public async Task GetList_Devnet_ReturnEmpty()
    {
        // Arrange
        SetupClient(NetworkType.DEVNET);

        // Act
        var result = await _supportedContractsModule.GetList();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetList_Testnet_VerifyCorrectRequest()
    {
        // Arrange
        SetupClient(NetworkType.TESTNET);
        _httpMessageHandlerMock.SetupAnyRequest().ReturnsResponse(HttpStatusCode.OK, "");

        // Act
        await _supportedContractsModule.GetList();

        // Assert
        _httpMessageHandlerMock.VerifyRequest(HttpMethod.Get, new Uri($"{BaseAddress}/SupportedContracts/list?networkType={(int)CirrusNetworkType.Testnet}"), Times.Once());
    }

    [Fact]
    public async Task GetList_Mainnet_VerifyCorrectRequest()
    {
        // Arrange
        SetupClient(NetworkType.MAINNET);
        _httpMessageHandlerMock.SetupAnyRequest().ReturnsResponse(HttpStatusCode.OK, "");

        // Act
        await _supportedContractsModule.GetList();

        // Assert
        _httpMessageHandlerMock.VerifyRequest(HttpMethod.Get, new Uri($"{BaseAddress}/SupportedContracts/list?networkType={(int)CirrusNetworkType.Mainnet}"), Times.Once());
    }

    [Fact]
    public async Task GetList_ValidResponse_ReturnDeserialized()
    {
        // Arrange
        var interfluxMappings = new InterfluxMappingDto[]
        {
            new()
            {
                TokenName = "ChainLink",
                NativeAddress = "nananana",
                NativeNetwork = NativeChainType.Ethereum,
                Src20Address = new Address("PUEcYJGdgj4QdEYTwEANsWnGKjmaS4NhcX")
            }
        };

        SetupClient(NetworkType.MAINNET);
        var responseContent = new StringContent(JsonConvert.SerializeObject(interfluxMappings, StratisFullNode.SerializerSettings));
        _httpMessageHandlerMock.SetupAnyRequest().ReturnsResponse(HttpStatusCode.OK, responseContent);

        // Act
        var results = await _supportedContractsModule.GetList();

        // Assert
        results.Should().BeEquivalentTo(interfluxMappings);
    }

    private void SetupClient(NetworkType networkType)
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        var httpClient = _httpMessageHandlerMock.CreateClient();
        httpClient.BaseAddress = new Uri(BaseAddress);

        _supportedContractsModule = new SupportedContractsModule(httpClient, NullLogger<SupportedContractsModule>.Instance, new OpdexConfiguration { Network = networkType });
    }
}
