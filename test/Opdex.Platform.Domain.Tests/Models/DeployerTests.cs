using System;
using System.Dynamic;
using FluentAssertions;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Domain.Models.TransactionLogs.MarketDeployers;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models
{
    public class DeployerTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void CreateDeployer_InvalidAddress_ThrowArgumentNullException(string address)
        {
            // Arrange
            const bool isActive = true;

            // Act
            void Act() => new Deployer(address, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", isActive, 100_000);

            // Assert
            Assert.Throws<ArgumentNullException>(Act);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void CreateDeployer_InvalidOwner_ThrowArgumentNullException(string owner)
        {
            // Arrange
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
        public void SetOwner_PreviousModifiedBlock_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            const bool isActive = true;
            var deployer = new Deployer("PE7FiEUa8NG9Xh2WU8q87nq2KGFTtoSPBD", "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", isActive, 100_000);

            dynamic log = new ExpandoObject();
            log.from = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            log.to = "PR71udY85pAcNcitdDfzQevp6Zar9DizHM";

            // Act
            void Act() => deployer.SetOwner(new ClaimPendingDeployerOwnershipLog(log, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", 0), 99_999);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act);
        }

        [Fact]
        public void SetOwner_ValidArguments_SetProperties()
        {
            // Arrange
            const bool isActive = true;
            var deployer = new Deployer("PE7FiEUa8NG9Xh2WU8q87nq2KGFTtoSPBD", "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", isActive, 100_000);

            var updatedOwner = "PR71udY85pAcNcitdDfzQevp6Zar9DizHM";

            dynamic log = new ExpandoObject();
            log.from = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            log.to = updatedOwner;

            // Act
            deployer.SetOwner(new ClaimPendingDeployerOwnershipLog(log, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", 0), 100_001);

            // Assert
            deployer.Owner.Should().Be(updatedOwner);
        }
    }
}
