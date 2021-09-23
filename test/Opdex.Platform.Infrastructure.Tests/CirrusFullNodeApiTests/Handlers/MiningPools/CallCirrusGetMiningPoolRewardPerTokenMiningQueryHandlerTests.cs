using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Constants.SmartContracts;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.MiningPools;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.MiningPools;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.CirrusFullNodeApiTests.Handlers.MiningPools
{
    public class CallCirrusGetMiningPoolRewardPerTokenMiningQueryHandlerTests
    {
        private readonly Mock<ISmartContractsModule> _smartContractsModuleMock;
        private readonly CallCirrusGetMiningPoolRewardPerTokenMiningQueryHandler _handler;

        public CallCirrusGetMiningPoolRewardPerTokenMiningQueryHandlerTests()
        {
            _smartContractsModuleMock = new Mock<ISmartContractsModule>();

            _handler = new CallCirrusGetMiningPoolRewardPerTokenMiningQueryHandler(_smartContractsModuleMock.Object);
        }

        [Fact]
        public void CallCirrusGetMiningPoolRewardPerTokenMiningQuery_InvalidMiningPool_ThrowsArgumentNullException()
        {
            // Arrange
            Address miningPool = Address.Empty;
            const ulong blockHeight = 10;

            // Act
            void Act() => new CallCirrusGetMiningPoolRewardPerTokenMiningQuery(miningPool, blockHeight);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Contains("Mining pool address must be provided.");
        }

        [Fact]
        public void CallCirrusGetMiningPoolRewardPerTokenMiningQuery_InvalidBlockHeight_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            Address miningPool = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            const ulong blockHeight = 0;

            // Act
            void Act() => new CallCirrusGetMiningPoolRewardPerTokenMiningQuery(miningPool, blockHeight);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("Block height must be greater than zero.");
        }

        [Fact]
        public async Task CallCirrusGetMiningPoolRewardPerTokenMiningQuery_Sends_LocalCallAsync()
        {
            // Arrange
            Address miningPool = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            const ulong blockHeight = 10;
            const string methodName = MiningPoolConstants.Methods.GetRewardPerStakedToken;
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            try
            {
                await _handler.Handle(new CallCirrusGetMiningPoolRewardPerTokenMiningQuery(miningPool, blockHeight), cancellationToken);
            }
            catch (Exception) { }

            // Assert
            _smartContractsModuleMock.Verify(callTo => callTo.LocalCallAsync(It.Is<LocalCallRequestDto>(q => q.ContractAddress == miningPool &&
                                                                                                             q.MethodName == methodName &&
                                                                                                             q.BlockHeight == blockHeight),
                                                                             cancellationToken), Times.Once);
        }

        [Fact]
        public async Task CallCirrusGetMiningPoolRewardPerTokenMiningQuery_Returns()
        {
            // Arrange
            Address miningPool = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            const ulong blockHeight = 10;
            var cancellationToken = new CancellationTokenSource().Token;
            UInt256 returnValue = 100;

            _smartContractsModuleMock.Setup(callTo => callTo.LocalCallAsync(It.IsAny<LocalCallRequestDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new LocalCallResponseDto { Return = returnValue });

            // Act
            var response = await _handler.Handle(new CallCirrusGetMiningPoolRewardPerTokenMiningQuery(miningPool, blockHeight), cancellationToken);

            // Assert
            response.Should().Be(returnValue);
        }
    }
}
