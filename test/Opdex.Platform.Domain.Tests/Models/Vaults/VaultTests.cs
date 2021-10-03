using FluentAssertions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.TransactionLogs.Vaults;
using Opdex.Platform.Domain.Models.Transactions;
using Opdex.Platform.Domain.Models.Vaults;
using System;
using System.Dynamic;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.Vaults
{
    public class VaultTests
    {
        [Fact]
        public void SetPendingOwnership_NullValueProvided_ThrowArgumentNullException()
        {
            // Arrange
            var vault = new Vault(5, "PJK7skDpACVauUvuqjBf7LXaWgRKCvMJL7", 5, "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy", "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", 5, 100000, 500, 505);
            var pendingOwner = "PTdjXpRFWXrUK7FCHcAjbsPWXaCSefipxh";

            dynamic log = new ExpandoObject();
            log.from = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            log.to = pendingOwner;

            // Act
            void Act() => vault.SetPendingOwnership(null, 510);

            // Assert
            Assert.Throws<ArgumentNullException>(Act);
        }


        [Fact]
        public void SetPendingOwnership_PendingOwner_Updated()
        {
            // Arrange
            var vault = new Vault(5, "PJK7skDpACVauUvuqjBf7LXaWgRKCvMJL7", 5, "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy", "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", 5, 100000, 500, 505);
            var pendingOwner = "PTdjXpRFWXrUK7FCHcAjbsPWXaCSefipxh";

            dynamic log = new ExpandoObject();
            log.from = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            log.to = pendingOwner;

            // Act
            vault.SetPendingOwnership(new SetPendingVaultOwnershipLog(log, "PAdS3HnzJ5QhacRuQ5Yb5koAp4XxqswnXi", 5), 510);

            // Assert
            vault.PendingOwner.Should().Be(pendingOwner);
        }

        [Fact]
        public void SetPendingOwnership_ModifiedBlock_Updated()
        {
            // Arrange
            var vault = new Vault(5, "PJK7skDpACVauUvuqjBf7LXaWgRKCvMJL7", 5, "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy", "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", 5, 100000, 500, 505);
            var pendingOwner = "PTdjXpRFWXrUK7FCHcAjbsPWXaCSefipxh";

            dynamic log = new ExpandoObject();
            log.from = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            log.to = pendingOwner;
            var blockHeight = 550UL;

            // Act
            vault.SetPendingOwnership(new SetPendingVaultOwnershipLog(log, "PAdS3HnzJ5QhacRuQ5Yb5koAp4XxqswnXi", 5), blockHeight);

            // Assert
            vault.ModifiedBlock.Should().Be(blockHeight);
        }

        [Fact]
        public void SetOwnershipClaimed_NullValueProvided_ThrowArgumentNullException()
        {
            // Arrange
            var vault = new Vault(5, "PJK7skDpACVauUvuqjBf7LXaWgRKCvMJL7", 5, "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy", "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", 5, 100000, 500, 505);
            var owner = "PTdjXpRFWXrUK7FCHcAjbsPWXaCSefipxh";

            dynamic log = new ExpandoObject();
            log.from = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            log.to = owner;

            // Act
            void Act() => vault.SetOwnershipClaimed(null, 510);

            // Assert
            Assert.Throws<ArgumentNullException>(Act);
        }

        [Fact]
        public void SetOwnershipClaimed_PreviousModifiedBlock_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            var currentOwner = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            var pendingOwner = "PR71udY85pAcNcitdDfzQevp6Zar9DizHM";
            var vault = new Vault(5, "PJK7skDpACVauUvuqjBf7LXaWgRKCvMJL7", 5, Address.Empty, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", 5, 100000, 500, 505);

            dynamic log = new ExpandoObject();
            log.from = currentOwner;
            log.to = pendingOwner;

            // Act
            void Act() => vault.SetOwnershipClaimed(new ClaimPendingVaultOwnershipLog(log, "PAdS3HnzJ5QhacRuQ5Yb5koAp4XxqswnXi", 5), 504);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act);
        }

        [Fact]
        public void SetOwnershipClaimed_ValidArguments_SetProperties()
        {
            // Arrange
            var pendingOwner = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
            var currentOwner = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            var vault = new Vault(5, "PJK7skDpACVauUvuqjBf7LXaWgRKCvMJL7", 5, pendingOwner, currentOwner, 5, 100000, 500, 505);

            dynamic log = new ExpandoObject();
            log.from = currentOwner;
            log.to = pendingOwner;
            ulong blockHeight = 510;

            // Act
            vault.SetOwnershipClaimed(new ClaimPendingVaultOwnershipLog(log, "PAdS3HnzJ5QhacRuQ5Yb5koAp4XxqswnXi", 5), blockHeight);

            // Assert
            vault.PendingOwner.Should().Be(Address.Empty);
            vault.Owner.Should().Be(pendingOwner);
            vault.ModifiedBlock.Should().Be(blockHeight);
        }

        [Fact]
        public void Update_Values_Updated()
        {
            // Arrange
            var vault = new Vault(5, "PJK7skDpACVauUvuqjBf7LXaWgRKCvMJL7", 5, "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy", "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", 5, 100000, 500, 505);

            const ulong block = 99999999;
            UInt256 updatedSupply = 100001;
            Address updatedPendingOwner = "PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh";
            Address updatedOwner = "Pf7LXaWgRKCvMJL7JK7skDpACVauUvuqjB";
            const ulong updatedGenesis = 10;

            var summary = new VaultContractSummary(block);
            summary.SetPendingOwner(new SmartContractMethodParameter(updatedPendingOwner));
            summary.SetOwner(new SmartContractMethodParameter(updatedOwner));
            summary.SetGenesis(new SmartContractMethodParameter(updatedGenesis));
            summary.SetUnassignedSupply(new SmartContractMethodParameter(updatedSupply));


            // Act
            vault.Update(summary);

            // Assert
            vault.UnassignedSupply.Should().Be(updatedSupply);
            vault.PendingOwner.Should().Be(updatedPendingOwner);
            vault.Owner.Should().Be(updatedOwner);
            vault.Genesis.Should().Be(updatedGenesis);
            vault.ModifiedBlock.Should().Be(block);
        }
    }
}
