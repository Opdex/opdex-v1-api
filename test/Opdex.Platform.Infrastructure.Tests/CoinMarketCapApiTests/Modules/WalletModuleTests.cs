using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moq.Contrib.HttpClient;
using Newtonsoft.Json;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Serialization;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.CoinMarketCapApiTests.Modules;

public class WalletModuleTests
{
    private const string BaseAddress = "https://cirrus.opdex.com/api/";
    private readonly Mock<HttpMessageHandler> _handler;
    private readonly WalletModule _walletModule;

    public WalletModuleTests()
    {
        _handler = new Mock<HttpMessageHandler>();
        var logger = NullLogger<WalletModule>.Instance;

        var httpClient = _handler.CreateClient();
        httpClient.BaseAddress = new Uri(BaseAddress);

        _walletModule = new WalletModule(httpClient, logger);
    }

    [Fact]
    public async Task VerifyMessage_SendRequest()
    {
        // arrange
        var request = new VerifyMessageRequestDto("MESSAGE_TO_SIGN", new Address("PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV"), "BASE64_SIGNATURE");
        _handler.SetupAnyRequest().ReturnsResponse(HttpStatusCode.OK, "\"True\"");

        // act
        await _walletModule.VerifyMessage(request, CancellationToken.None);

        // assert
        _handler.VerifyRequest(HttpMethod.Post, $"{BaseAddress}Wallet/verifymessage", async httpRequestMessage =>
        {
            var rawBody = await httpRequestMessage.Content.ReadAsStringAsync();
            var body = JsonConvert.DeserializeObject<VerifyMessageRequestDto>(rawBody, StratisFullNode.SerializerSettings);
            return body.Message == request.Message && body.ExternalAddress == request.ExternalAddress && body.Signature == request.Signature;
        });
    }
}