using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Constants.SmartContracts.Tokens;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Tokens;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.CirrusFullNodeApiTests.Handlers.Tokens
{
    public class CallCirrusGetSrcTokenBalanceQueryHandlerTests
    {
        private readonly Mock<ISmartContractsModule> _smartContractsModuleMock;
        private readonly CallCirrusGetSrcTokenBalanceQueryHandler _handler;

        public CallCirrusGetSrcTokenBalanceQueryHandlerTests()
        {
            _smartContractsModuleMock = new Mock<ISmartContractsModule>();

            _handler = new CallCirrusGetSrcTokenBalanceQueryHandler(_smartContractsModuleMock.Object);
        }

        [Fact]
        public void CallCirrusGetSrcTokenBalanceQuery_InvalidTokenAddress_ThrowsArgumentNullException()
        {
            // Arrange
            Address owner = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            const ulong blockHeight = 10;

            // Act
            void Act() => new CallCirrusGetSrcTokenBalanceQuery(null, owner, blockHeight);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Contains("Token address must be provided.");
        }

        [Fact]
        public void CallCirrusGetSrcTokenBalanceQuery_InvalidOwnerAddress_ThrowsArgumentNullException()
        {
            // Arrange
            Address token = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            const ulong blockHeight = 10;

            // Act
            void Act() => new CallCirrusGetSrcTokenBalanceQuery(token, null, blockHeight);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Contains("Owner address must be provided.");
        }

        [Fact]
        public void CallCirrusGetSrcTokenBalanceQuery_InvalidBlockHeight_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            Address token = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            Address owner = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";

            // Act
            void Act() => new CallCirrusGetSrcTokenBalanceQuery(token, owner, 0);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("Block height must be greater than zero.");
        }

        [Fact]
        public async Task CallCirrusGetSrcTokenBalanceQuery_Sends_LocalCallAsync()
        {
            // Arrange
            Address token = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            Address owner = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            const ulong blockHeight = 10;
            const string methodName = StandardTokenConstants.Methods.GetBalance;

            var request = new CallCirrusGetSrcTokenBalanceQuery(token, owner, blockHeight);
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            try
            {
                await _handler.Handle(request, cancellationToken);
            }
            catch (Exception) { }

            // Assert
            _smartContractsModuleMock.Verify(callTo => callTo.LocalCallAsync(It.Is<LocalCallRequestDto>(q => q.ContractAddress == token
                                                                                                             && q.MethodName == methodName
                                                                                                             && q.BlockHeight == blockHeight
                                                                             ), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task CallCirrusGetSrcTokenBalanceQuery_Returns()
        {
            // Arrange
            Address token = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            Address owner = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            const ulong blockHeight = 10;
            UInt256 returnValue = UInt256.Parse("500000000");

            _smartContractsModuleMock.Setup(callTo => callTo.LocalCallAsync(It.IsAny<LocalCallRequestDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new LocalCallResponseDto { Return = returnValue });

            // Act
            var response = await _handler.Handle(new CallCirrusGetSrcTokenBalanceQuery(token, owner, blockHeight), CancellationToken.None);

            // Assert
            response.Should().Be(returnValue);
        }
    }
}
