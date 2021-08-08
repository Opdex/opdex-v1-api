using System;
using FluentAssertions;
using Opdex.Platform.Domain.Models.Markets;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.Markets
{
    public class MarketTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void CreateMarket_InvalidAddress_ThrowArgumentNullException(string address)
        {
            // Arrange
            // Act
            void Act() => new Market(address, 5, 10, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", true, true, true, 3, true, 100_000);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Address must be set.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void CreateMarket_InvalidOwner_ThrowArgumentNullException(string owner)
        {
            // Arrange
            // Act
            void Act() => new Market("PMWrLGcwhr1zboamZQzC5Jk75JyYJSAzoi", 5, 10, owner, true, true, true, 3, true, 100_000);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Owner must be set.");
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void CreateMarket_InvalidDeployerId_ThrowArgumentOutOfRangeException(long deployerId)
        {
            // Arrange
            // Act
            void Act() => new Market("PMWrLGcwhr1zboamZQzC5Jk75JyYJSAzoi", deployerId, 10, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", true, true, true, 3, true, 100_000);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Deployer id must be greater than 0.");
        }

        [Fact]
        public void CreateMarket_InvalidTransactionFee_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            uint transactionFee = 11;

            // Act
            void Act() => new Market("PMWrLGcwhr1zboamZQzC5Jk75JyYJSAzoi", 1, 10, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", true, true, true, transactionFee, true, 100_000);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Transaction fee must be between 0-10 inclusive.");
        }

        [Fact]
        public void CreateMarket_ValidArguments_PropertiesAreSet()
        {
            // Arrange
            var address = "PMWrLGcwhr1zboamZQzC5Jk75JyYJSAzoi";
            var deployerId = 5;
            var stakingTokenId = 10;
            var owner = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            var authPoolCreators = true;
            var authProviders = true;
            var authTraders = true;
            uint transactionFee = 3;
            var marketFeeEnabled = true;
            ulong createdBlock = 100_000;

            // Act
            var market = new Market(address, deployerId, stakingTokenId, owner, authPoolCreators, authProviders, authTraders, transactionFee, marketFeeEnabled, createdBlock);

            // Assert
            market.Address.Should().Be(address);
            market.DeployerId.Should().Be(deployerId);
            market.StakingTokenId.Should().Be(stakingTokenId);
            market.Owner.Should().Be(owner);
            market.AuthPoolCreators.Should().Be(authPoolCreators);
            market.AuthProviders.Should().Be(authProviders);
            market.AuthTraders.Should().Be(authTraders);
            market.TransactionFee.Should().Be(transactionFee);
            market.MarketFeeEnabled.Should().Be(marketFeeEnabled);
            market.CreatedBlock.Should().Be(createdBlock);
            market.ModifiedBlock.Should().Be(createdBlock);
        }
    }
}