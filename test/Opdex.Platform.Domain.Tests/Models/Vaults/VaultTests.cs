using FluentAssertions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.TransactionLogs.Vaults;
using Opdex.Platform.Domain.Models.Transactions;
using Opdex.Platform.Domain.Models.Vaults;
using System.Dynamic;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.Vaults
{
    public class VaultTests
    {
        [Fact]
        public void SetOwner_Owner_Updated()
        {
            // Arrange
            var vault = new Vault(5, "PJK7skDpACVauUvuqjBf7LXaWgRKCvMJL7", 5, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", 5, 100000, 500, 505);
            var owner = "PTdjXpRFWXrUK7FCHcAjbsPWXaCSefipxh";

            dynamic log = new ExpandoObject();
            log.from = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            log.to = owner;

            // Act
            vault.SetOwner(new ClaimPendingVaultOwnershipLog(log, "PAdS3HnzJ5QhacRuQ5Yb5koAp4XxqswnXi", 5), 510);

            // Assert
            vault.Owner.Should().Be(owner);
        }

        [Fact]
        public void SetOwner_ModifiedBlock_Updated()
        {
            // Arrange
            var vault = new Vault(5, "PJK7skDpACVauUvuqjBf7LXaWgRKCvMJL7", 5, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", 5, 100000, 500, 505);
            var blockHeight = 510UL;

            dynamic log = new ExpandoObject();
            log.from = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            log.to = "PTdjXpRFWXrUK7FCHcAjbsPWXaCSefipxh";

            // Act
            vault.SetOwner(new ClaimPendingVaultOwnershipLog(log, "PAdS3HnzJ5QhacRuQ5Yb5koAp4XxqswnXi", 5), blockHeight);

            // Assert
            vault.ModifiedBlock.Should().Be(blockHeight);
        }

        [Fact]
        public void SetUnassignedSupply_UnassignedSupply_Updated()
        {
            // Arrange
            var vault = new Vault(5, "PJK7skDpACVauUvuqjBf7LXaWgRKCvMJL7", 5, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", 5, 100000, 500, 505);

            const ulong block = 99999999;
            UInt256 updatedSupply = 100001;
            Address updatedOwner = "Pf7LXaWgRKCvMJL7JK7skDpACVauUvuqjB";
            const ulong updatedGenesis = 10;

            var summary = new VaultContractSummary(block);
            summary.SetOwner(new SmartContractMethodParameter(updatedOwner));
            summary.SetGenesis(new SmartContractMethodParameter(updatedGenesis));
            summary.SetUnassignedSupply(new SmartContractMethodParameter(updatedSupply));


            // Act
            vault.Update(summary);

            // Assert
            vault.UnassignedSupply.Should().Be(updatedSupply);
            vault.Owner.Should().Be(updatedOwner);
            vault.Genesis.Should().Be(updatedGenesis);
            vault.ModifiedBlock.Should().Be(block);
        }
    }
}
