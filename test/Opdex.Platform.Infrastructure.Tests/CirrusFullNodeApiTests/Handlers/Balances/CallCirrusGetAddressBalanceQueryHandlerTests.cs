using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Balances;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Balances;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.CirrusFullNodeApiTests.Handlers.Balances
{
    public class CallCirrusGetAddressBalanceQueryHandlerTests
    {
        private readonly Mock<ISmartContractsModule> _smartContractsModuleMock;
        private CallCirrusGetAddressBalanceQueryHandler _handler;

        public CallCirrusGetAddressBalanceQueryHandlerTests()
        {
            _smartContractsModuleMock = new Mock<ISmartContractsModule>();
        }

        // We can't mock OpdexConfiguration unless we create an interface and make adjustments just for tests.
        // Opting for a reusable method to create the handler during the arrangement of each test.
        private void SetupHandler(NetworkType networkType)
        {
            var configuration = new OpdexConfiguration { Network = networkType };
            _handler = new CallCirrusGetAddressBalanceQueryHandler(configuration, _smartContractsModuleMock.Object);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void CallCirrusGetAddressBalance_ThrowsArgumentNullException_InvalidAddress(string address)
        {
            // Arrange
            // Act
            void Act() => new CallCirrusGetAddressBalanceQuery(address);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("A wallet address must be provided.");
        }

        [Fact]
        public async Task CallCirrusGetAddressBalance_Devnet_Sends_GetWalletAddressCrsBalance()
        {
            // Arrange
            Address wallet = "P9F65J5Nk6AxVkbJSw9iohk8zniD3waiRf";
            var token = new CancellationTokenSource().Token;
            SetupHandler(NetworkType.DEVNET);

            // Act
            try
            {
                await _handler.Handle(new CallCirrusGetAddressBalanceQuery(wallet.ToString()), token);
            }
            catch { }

            // Assert
            _smartContractsModuleMock.Verify(callTo => callTo.GetWalletAddressCrsBalance(wallet.ToString(), token), Times.Once);
        }

        [Fact]
        public async Task CallCirrusGetAddressBalance_Devnet_Returns()
        {
            // Arrange
            Address wallet = "P9F65J5Nk6AxVkbJSw9iohk8zniD3waiRf";
            var token = new CancellationTokenSource().Token;
            const ulong balance = 10;
            SetupHandler(NetworkType.DEVNET);
            _smartContractsModuleMock.Setup(callTo => callTo.GetWalletAddressCrsBalance(wallet.ToString(), token)).ReturnsAsync(balance);

            // Act
            var response = await _handler.Handle(new CallCirrusGetAddressBalanceQuery(wallet.ToString()), token);

            // Assert
            response.Should().Be(balance);
        }

        [Theory]
        [InlineData(NetworkType.TESTNET)]
        [InlineData(NetworkType.MAINNET)]
        public async Task CallCirrusGetAddressBalance_TestAndMainnet_ReturnsZero(NetworkType networkType)
        {
            // Arrange
            Address wallet = "P9F65J5Nk6AxVkbJSw9iohk8zniD3waiRf";
            var token = new CancellationTokenSource().Token;
            SetupHandler(networkType);

            // Act
            var response = await _handler.Handle(new CallCirrusGetAddressBalanceQuery(wallet.ToString()), token);

            // Assert
            response.Should().Be(0ul);
        }

        [Fact]
        public void CallCirrusGetAddressBalance_CaughtException_FindOrThrowTrue_Throws()
        {
            // Arrange
            Address wallet = "P9F65J5Nk6AxVkbJSw9iohk8zniD3waiRf";
            var token = new CancellationTokenSource().Token;
            SetupHandler(NetworkType.DEVNET);
            _smartContractsModuleMock
                .Setup(callTo => callTo.GetWalletAddressCrsBalance(wallet.ToString(), token))
                .Throws(new Exception());

            // Act
            // Assert
            _handler
                .Invoking(h => h.Handle(new CallCirrusGetAddressBalanceQuery(wallet.ToString(), findOrThrow: true), token))
                .Should()
                .Throw<Exception>();
        }

        [Fact]
        public async Task CallCirrusGetAddressBalance_CaughtException_FindOrThrowFalse_ReturnsZero()
        {
            // Arrange
            Address wallet = "P9F65J5Nk6AxVkbJSw9iohk8zniD3waiRf";
            var token = new CancellationTokenSource().Token;
            SetupHandler(NetworkType.DEVNET);
            _smartContractsModuleMock
                .Setup(callTo => callTo.GetWalletAddressCrsBalance(wallet.ToString(), token))
                .Throws(new Exception());

            // Act
            var response = await _handler.Handle(new CallCirrusGetAddressBalanceQuery(wallet.ToString(), findOrThrow: false), token);

            // Assert
            response.Should().Be(0);
        }
    }
}
