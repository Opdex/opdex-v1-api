using System;
using System.Dynamic;
using FluentAssertions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Domain.Models.TransactionLogs.MarketDeployers;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models
{
    public class DeployerTests
    {
        [Fact]
        public void CreateDeployer_InvalidAddress_ThrowArgumentNullException()
        {
            // Arrange
            var address = Address.Empty;
            const bool isActive = true;

            // Act
            void Act() => new Deployer(address, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", isActive, 100_000);

            // Assert
            Assert.Throws<ArgumentNullException>(Act);
        }

        [Fact]
        public void CreateDeployer_InvalidOwner_ThrowArgumentNullException()
        {
            // Arrange
            var owner = Address.Empty;
            const bool isActive = true;

            // Act
            void Act() => new Deployer("PE7FiEUa8NG9Xh2WU8q87nq2KGFTtoSPBD", owner, isActive, 100_000);

            // Assert
            Assert.Throws<ArgumentNullException>(Act);
        }

        [Fact]
        public void CreateDeployer_ValidArguments_SetProperties()
        {
            // Arrange
            var address = "PE7FiEUa8NG9Xh2WU8q87nq2KGFTtoSPBD";
            var owner = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            ulong createdBlock = 100_000;
            const bool isActive = true;

            // Act
            var deployer = new Deployer(address, owner, isActive, createdBlock);

            // Assert
            deployer.Address.Should().Be(address);
            deployer.Owner.Should().Be(owner);
            deployer.IsActive.Should().Be(isActive);
            deployer.CreatedBlock.Should().Be(createdBlock);
            deployer.ModifiedBlock.Should().Be(createdBlock);
        }

        [Fact]
        public void SetOwner_InvalidADdress_ThrowArgumentNullException()
        {
            // Arrange
            var deployer = new Deployer("PE7FiEUa8NG9Xh2WU8q87nq2KGFTtoSPBD", "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", true, 100_000);

            // Act
            void Act() => deployer.SetOwner(Address.Empty, 99_999);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Contains("Owner address must be provided.");
        }

        [Fact]
        public void SetOwner_PreviousModifiedBlock_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            const bool isActive = true;
            Address currentOwner = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            Address newOwner = "PR71udY85pAcNcitdDfzQevp6Zar9DizHM";
            var deployer = new Deployer("PE7FiEUa8NG9Xh2WU8q87nq2KGFTtoSPBD", currentOwner, isActive, 100_000);

            // Act
            void Act() => deployer.SetOwner(newOwner, 99_999);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act);
        }

        [Theory]
        [InlineData(100_000)]
        [InlineData(100_001)]
        public void SetOwner_ValidArguments_SetProperties(ulong currentBlock)
        {
            // Arrange
            const bool isActive = true;
            Address currentOwner = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            Address newOwner = "PR71udY85pAcNcitdDfzQevp6Zar9DizHM";
            var deployer = new Deployer("PE7FiEUa8NG9Xh2WU8q87nq2KGFTtoSPBD", currentOwner, isActive, 100_000);

            // Act
            deployer.SetOwner(newOwner, currentBlock);

            // Assert
            deployer.Owner.Should().Be(newOwner);
        }
    }
}
