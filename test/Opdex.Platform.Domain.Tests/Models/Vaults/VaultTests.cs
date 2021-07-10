using FluentAssertions;
using Opdex.Platform.Domain.Models.ODX;
using Opdex.Platform.Domain.Models.TransactionLogs.Vaults;
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
            var vault = new Vault(5, "PJK7skDpACVauUvuqjBf7LXaWgRKCvMJL7", 5, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", 5, "100000", 500, 505);
            var owner = "PTdjXpRFWXrUK7FCHcAjbsPWXaCSefipxh";

            dynamic log = new ExpandoObject();
            log.from = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            log.to = owner;

            // Act
            vault.SetOwner(new ChangeVaultOwnerLog(log, "PAdS3HnzJ5QhacRuQ5Yb5koAp4XxqswnXi", 5), 510);

            // Assert
            vault.Owner.Should().Be(owner);
        }

        [Fact]
        public void SetOwner_ModifiedBlock_Updated()
        {
            // Arrange
            var vault = new Vault(5, "PJK7skDpACVauUvuqjBf7LXaWgRKCvMJL7", 5, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", 5, "100000", 500, 505);
            var blockHeight = 510UL;

            dynamic log = new ExpandoObject();
            log.from = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            log.to = "PTdjXpRFWXrUK7FCHcAjbsPWXaCSefipxh";

            // Act
            vault.SetOwner(new ChangeVaultOwnerLog(log, "PAdS3HnzJ5QhacRuQ5Yb5koAp4XxqswnXi", 5), blockHeight);

            // Assert
            vault.ModifiedBlock.Should().Be(blockHeight);
        }

        [Fact]
        public void SetUnassignedSupply_UnassignedSupply_Updated()
        {
            // Arrange
            var vault = new Vault(5, "PJK7skDpACVauUvuqjBf7LXaWgRKCvMJL7", 5, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", 5, "100000", 500, 505);
            var updatedSupply = "100001";

            // Act
            vault.SetUnassignedSupply(updatedSupply, 510);

            // Assert
            vault.UnassignedSupply.Should().Be(updatedSupply);
        }

        [Fact]
        public void SetUnassignedSupply_ModifiedBlock_Updated()
        {
            // Arrange
            var vault = new Vault(5, "PJK7skDpACVauUvuqjBf7LXaWgRKCvMJL7", 5, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", 5, "100000", 500, 505);
            var blockHeight = 510UL;

            // Act
            vault.SetUnassignedSupply("100001", blockHeight);

            // Assert
            vault.ModifiedBlock.Should().Be(blockHeight);
        }
    }
}
