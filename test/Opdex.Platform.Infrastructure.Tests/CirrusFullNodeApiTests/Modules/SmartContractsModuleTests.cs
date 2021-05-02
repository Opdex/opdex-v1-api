using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Modules;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.CirrusFullNodeApiTests.Modules
{
    public class SmartContractsModuleTests
    {
        private readonly ISmartContractsModule _smartContractModule;

        public SmartContractsModuleTests()
        {
            var handler = new HttpClientHandler();
            _smartContractModule = new SmartContractsModule(new HttpClient(handler), NullLogger<SmartContractsModule>.Instance);
        }

        [Fact]
        public async Task ReceiptSearch_Success()
        {
            // arrange
            const string contract = "tDrNbZKsbYPvike4RfddzESXZoPwUMm5pL";
            const string logName = "SwapLog";

            // act
            var response = await _smartContractModule.ReceiptSearchAsync(contract, logName, 10, 10000, CancellationToken.None);

            // assert
            Assert.NotNull(response);
            Assert.True(response.Any());
        }
    }
}