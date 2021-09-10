using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moq.Contrib.HttpClient;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Modules;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.CirrusFullNodeApiTests.Modules
{
    public class SmartContractsModuleTests
    {
        [Fact]
        public async Task ReceiptSearch_WithoutToBlock_SendRequest()
        {
            // arrange
            const string baseAddress = "https://cirrus.node/";
            const string contract = "tDrNbZKsbYPvike4RfddzESXZoPwUMm5pL";
            const string logName = "SwapLog";
            const int fromBlock = 10;

            var handler = new Mock<HttpMessageHandler>();
            handler.SetupAnyRequest().ReturnsResponse(HttpStatusCode.OK, "");
            var httpClient = handler.CreateClient();
            httpClient.BaseAddress = new Uri(baseAddress);

            var smartContractsModule = new SmartContractsModule(httpClient, NullLogger<SmartContractsModule>.Instance);

            // act
            await smartContractsModule.ReceiptSearchAsync(contract, logName, fromBlock, null, CancellationToken.None);

            // assert
            handler.VerifyRequest(HttpMethod.Get,
                                  "https://cirrus.node/SmartContracts/receipt-search?contractAddress=tDrNbZKsbYPvike4RfddzESXZoPwUMm5pL&eventName=SwapLog&fromBlock=10");
        }

        [Fact]
        public async Task ReceiptSearch_WithToBlock_SendRequest()
        {
            // arrange
            const string baseAddress = "https://cirrus.node/";
            const string contract = "tDrNbZKsbYPvike4RfddzESXZoPwUMm5pL";
            const string logName = "SwapLog";
            const int fromBlock = 10;
            const int toBlock = 10000;

            var handler = new Mock<HttpMessageHandler>();
            handler.SetupAnyRequest().ReturnsResponse(HttpStatusCode.OK, "");
            var httpClient = handler.CreateClient();
            httpClient.BaseAddress = new Uri(baseAddress);

            var smartContractsModule = new SmartContractsModule(httpClient, NullLogger<SmartContractsModule>.Instance);

            // act
            await smartContractsModule.ReceiptSearchAsync(contract, logName, fromBlock, toBlock, CancellationToken.None);

            // assert
            handler.VerifyRequest(HttpMethod.Get,
                                  "https://cirrus.node/SmartContracts/receipt-search?contractAddress=tDrNbZKsbYPvike4RfddzESXZoPwUMm5pL&eventName=SwapLog&fromBlock=10&to=10000");
        }
    }
}
