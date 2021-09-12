using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Vaults;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Vaults;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.CirrusFullNodeApiTests.Handlers.Vaults
{
    public class CallCirrusGetVaultTotalSupplyQueryHandlerTests
    {
        private readonly Mock<ISmartContractsModule> _smartContractsModuleMock;
        private readonly CallCirrusGetVaultTotalSupplyQueryHandler _handler;

        public CallCirrusGetVaultTotalSupplyQueryHandlerTests()
        {
            _smartContractsModuleMock = new Mock<ISmartContractsModule>();

            _handler = new CallCirrusGetVaultTotalSupplyQueryHandler(_smartContractsModuleMock.Object);
        }

        [Fact]
        public async Task Handle_LocalCallAsync_Send()
        {
            // Arrange
            var contractAddress = "P9F65J5Nk6AxVkbJSw9iohk8zniD3waiRf";
            var blockHeight = 50000UL;
            var request = new CallCirrusGetVaultTotalSupplyQuery(contractAddress, blockHeight);
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            try
            {
                await _handler.Handle(request, cancellationToken);
            }
            catch (Exception) { }

            // Assert
            _smartContractsModuleMock.Verify(callTo => callTo.LocalCallAsync(It.Is<LocalCallRequestDto>(
                request => request.ContractAddress == contractAddress
                        && request.MethodName == "get_TotalSupply"
                        && request.BlockHeight == blockHeight
            ), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task Handle_LocalCallAsync_ReturnValue()
        {
            // Arrange
            var returnValue = "500000000";
            _smartContractsModuleMock.Setup(callTo => callTo.LocalCallAsync(It.IsAny<LocalCallRequestDto>(), It.IsAny<CancellationToken>()))
                                     .ReturnsAsync(new LocalCallResponseDto
                                     {
                                         Return = returnValue
                                     });

            // Act
            var response = await _handler.Handle(new CallCirrusGetVaultTotalSupplyQuery("P8zHy2c8Nydkh2r6Wv6K6kacxkDcZyfaLy", 500000), CancellationToken.None);

            // Assert
            response.Should().Be(UInt256.Parse(returnValue));
        }
    }
}
