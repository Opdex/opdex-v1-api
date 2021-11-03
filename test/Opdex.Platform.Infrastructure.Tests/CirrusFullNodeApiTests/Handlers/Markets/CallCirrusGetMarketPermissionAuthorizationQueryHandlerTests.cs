using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Constants.SmartContracts.Markets;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Markets;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Markets;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.CirrusFullNodeApiTests.Handlers.Markets
{
    public class CallCirrusGetMarketPermissionAuthorizationQueryHandlerTests
    {
        private readonly Mock<ISmartContractsModule> _smartContractsModuleMock;
        private readonly CallCirrusGetMarketPermissionAuthorizationQueryHandler _handler;

        public CallCirrusGetMarketPermissionAuthorizationQueryHandlerTests()
        {
            _smartContractsModuleMock = new Mock<ISmartContractsModule>();

            _handler = new CallCirrusGetMarketPermissionAuthorizationQueryHandler(_smartContractsModuleMock.Object);
        }

        [Fact]
        public void CallCirrusGetMarketPermissionAuthorizationQuery_InvalidMarket_ThrowsArgumentNullException()
        {
            // Arrange
            Address market = Address.Empty;
            Address wallet = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            const MarketPermissionType permission = MarketPermissionType.Provide;
            const ulong blockHeight = 10;

            // Act
            void Act() => new CallCirrusGetMarketPermissionAuthorizationQuery(market, wallet, permission, blockHeight);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Contains("Market address must be provided.");
        }

        [Fact]
        public void CallCirrusGetMarketPermissionAuthorizationQuery_InvalidWallet_ThrowsArgumentNullException()
        {
            // Arrange
            Address market = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            Address wallet = Address.Empty;
            const MarketPermissionType permission = MarketPermissionType.Provide;
            const ulong blockHeight = 10;

            // Act
            void Act() => new CallCirrusGetMarketPermissionAuthorizationQuery(market, wallet, permission, blockHeight);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Contains("Wallet address must be provided.");
        }

        [Fact]
        public void CallCirrusGetMarketPermissionAuthorizationQuery_InvalidPermissionType_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            Address market = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            Address wallet = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            const MarketPermissionType permission = 0;
            const ulong blockHeight = 10;

            // Act
            void Act() => new CallCirrusGetMarketPermissionAuthorizationQuery(market, wallet, permission, blockHeight);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("Permission type must be valid.");
        }

        [Fact]
        public void CallCirrusGetMarketPermissionAuthorizationQuery_InvalidBlockHeight_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            Address market = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            Address wallet = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            const MarketPermissionType permission = MarketPermissionType.Provide;
            const ulong blockHeight = 0;

            // Act
            void Act() => new CallCirrusGetMarketPermissionAuthorizationQuery(market, wallet, permission, blockHeight);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("Block height must be greater than zero.");
        }

        [Fact]
        public async Task CallCirrusGetMarketPermissionAuthorizationQuery_Sends_LocalCallAsync()
        {
            // Arrange
            Address market = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            Address wallet = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            const MarketPermissionType permission = MarketPermissionType.Provide;
            const ulong blockHeight = 10;
            const string methodName = StandardMarketConstants.Methods.IsAuthorized;
            var cancellationToken = new CancellationTokenSource().Token;

            var parameters = new[]
            {
                new SmartContractMethodParameter(wallet),
                new SmartContractMethodParameter((byte)permission)
            };

            // Act
            try
            {
                await _handler.Handle(new CallCirrusGetMarketPermissionAuthorizationQuery(market, wallet, permission, blockHeight), cancellationToken);
            }
            catch (Exception) { }

            // Assert
            _smartContractsModuleMock.Verify(callTo => callTo.LocalCallAsync(It.Is<LocalCallRequestDto>(q => q.ContractAddress == market
                                                                                                             && q.MethodName == methodName
                                                                                                             && q.BlockHeight == blockHeight
                                                                                                             && q.Parameters.All(p => parameters.Contains(p))
                                                                             ), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task CallCirrusGetMarketPermissionAuthorizationQuery_Returns()
        {
            // Arrange
            Address market = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            Address wallet = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            const MarketPermissionType permission = MarketPermissionType.Provide;
            const ulong blockHeight = 10;
            var cancellationToken = new CancellationTokenSource().Token;
            const bool returnValue = true;

            _smartContractsModuleMock.Setup(callTo => callTo.LocalCallAsync(It.IsAny<LocalCallRequestDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new LocalCallResponseDto { Return = returnValue });

            // Act
            var response = await _handler.Handle(new CallCirrusGetMarketPermissionAuthorizationQuery(market, wallet, permission, blockHeight), cancellationToken);

            // Assert
            response.Should().Be(returnValue);
        }
    }
}
