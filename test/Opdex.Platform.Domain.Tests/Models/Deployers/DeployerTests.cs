using System;
using System.Dynamic;
using FluentAssertions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Deployers;
using Opdex.Platform.Domain.Models.TransactionLogs.MarketDeployers;
using Opdex.Platform.Domain.Models.Transactions;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.Deployers
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
        public void SetPendingOwnership_NullValueProvided_ThrowArgumentNullException()
        {
            // Arrange
            var deployer = new Deployer("PE7FiEUa8NG9Xh2WU8q87nq2KGFTtoSPBD", "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", true, 100_000);
            var pendingOwner = "PTdjXpRFWXrUK7FCHcAjbsPWXaCSefipxh";

            dynamic log = new ExpandoObject();
            log.from = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            log.to = pendingOwner;

            // Act
            void Act() => deployer.SetPendingOwnership(null, 510);

            // Assert
            Assert.Throws<ArgumentNullException>(Act);
        }


        [Fact]
        public void SetPendingOwnership_PendingOwner_Updated()
        {
            // Arrange
            var deployer = new Deployer("PE7FiEUa8NG9Xh2WU8q87nq2KGFTtoSPBD", "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", true, 100_000);
            var pendingOwner = "PTdjXpRFWXrUK7FCHcAjbsPWXaCSefipxh";

            dynamic log = new ExpandoObject();
            log.from = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            log.to = pendingOwner;

            // Act
            deployer.SetPendingOwnership(new SetPendingDeployerOwnershipLog(log, "PAdS3HnzJ5QhacRuQ5Yb5koAp4XxqswnXi", 5), 100_005);

            // Assert
            deployer.PendingOwner.Should().Be(pendingOwner);
        }

        [Fact]
        public void SetPendingOwnership_ModifiedBlock_Updated()
        {
            // Arrange
            var deployer = new Deployer("PE7FiEUa8NG9Xh2WU8q87nq2KGFTtoSPBD", "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", true, 100_000);
            var pendingOwner = "PTdjXpRFWXrUK7FCHcAjbsPWXaCSefipxh";

            dynamic log = new ExpandoObject();
            log.from = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            log.to = pendingOwner;
            ulong blockHeight = 100_005;

            // Act
            deployer.SetPendingOwnership(new SetPendingDeployerOwnershipLog(log, "PAdS3HnzJ5QhacRuQ5Yb5koAp4XxqswnXi", 5), blockHeight);

            // Assert
            deployer.ModifiedBlock.Should().Be(blockHeight);
        }

        [Fact]
        public void SetOwnershipClaimed_NullValueProvided_ThrowArgumentNullException()
        {
            // Arrange
            var deployer = new Deployer("PE7FiEUa8NG9Xh2WU8q87nq2KGFTtoSPBD", "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", true, 100_000);

            // Act
            void Act() => deployer.SetOwnershipClaimed(null, 99_999);

            // Assert
            Assert.Throws<ArgumentNullException>(Act);
        }

        [Fact]
        public void SetOwnershipClaimed_PreviousModifiedBlock_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            var currentOwner = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            var pendingOwner = "PR71udY85pAcNcitdDfzQevp6Zar9DizHM";
            var deployer = new Deployer(5, "PE7FiEUa8NG9Xh2WU8q87nq2KGFTtoSPBD", pendingOwner, currentOwner, true, 100_000, 100_005);

            dynamic log = new ExpandoObject();
            log.from = currentOwner;
            log.to = pendingOwner;

            // Act
            void Act() => deployer.SetOwnershipClaimed(new ClaimPendingDeployerOwnershipLog(log, "PAdS3HnzJ5QhacRuQ5Yb5koAp4XxqswnXi", 5), 99_999);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act);
        }

        [Fact]
        public void SetOwnershipClaimed_ValidArguments_SetProperties()
        {
            // Arrange
            var currentOwner = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            var pendingOwner = "PR71udY85pAcNcitdDfzQevp6Zar9DizHM";
            var deployer = new Deployer(5, "PE7FiEUa8NG9Xh2WU8q87nq2KGFTtoSPBD", pendingOwner, currentOwner, true, 100_000, 100_005);

            dynamic log = new ExpandoObject();
            log.from = currentOwner;
            log.to = pendingOwner;
            ulong blockHeight = 100_010;

            // Act
            deployer.SetOwnershipClaimed(new ClaimPendingDeployerOwnershipLog(log, "PAdS3HnzJ5QhacRuQ5Yb5koAp4XxqswnXi", 5), blockHeight);

            // Assert
            deployer.PendingOwner.Should().Be(Address.Empty);
            deployer.Owner.Should().Be(pendingOwner);
            deployer.ModifiedBlock.Should().Be(blockHeight);
        }

        [Fact]
        public void Update_Values_Updated()
        {
            // Arrange
            var deployer = new Deployer(5, "PE7FiEUa8NG9Xh2WU8q87nq2KGFTtoSPBD", "PAdS3HnzJ5QhacRuQ5Yb5koAp4XxqswnXi", "PE7FiEUa8NG9Xh2WU8q87nq2KGFTtoSPBD", true, 100_000, 100_005);

            const ulong block = 99999999;
            Address updatedPendingOwner = "PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh";
            Address updatedOwner = "Pf7LXaWgRKCvMJL7JK7skDpACVauUvuqjB";

            var summary = new DeployerContractSummary(block);
            summary.SetPendingOwner(new SmartContractMethodParameter(updatedPendingOwner));
            summary.SetOwner(new SmartContractMethodParameter(updatedOwner));

            // Act
            deployer.Update(summary);

            // Assert
            deployer.PendingOwner.Should().Be(updatedPendingOwner);
            deployer.Owner.Should().Be(updatedOwner);
            deployer.ModifiedBlock.Should().Be(block);
        }
    }
}
