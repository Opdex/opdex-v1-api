using FluentAssertions;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Tokens;
using System;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.Tokens;

public class TokenDistributionTests
{
    [Fact]
    public void CreateTokenDistribution_InvalidTokenId_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        const ulong tokenId = 0;

        // Act
        static void Act() => new TokenDistribution(tokenId, 10, 20, 2, 3, 4, 5);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Token Id must be greater than 0.");
    }

    [Fact]
    public void CreateTokenDistribution_InvalidDistributionBlock_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        const ulong distributionBlock = 0;

        // Act
        static void Act() => new TokenDistribution(1, 10, 20, 2, distributionBlock, 4);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Distribution block must be greater than 0.");
    }

    [Fact]
    public void CreateTokenDistribution_InvalidNextDistributionBlock_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        const ulong nextDistributionBlock = 0;

        // Act
        static void Act() => new TokenDistribution(1, 10, 20, 2, 3, 4, nextDistributionBlock);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Next distribution block must be greater than 0.");
    }

    [Fact]
    public void CreatesNew_TokenDistribution_Success()
    {
        // Arrange
        const ulong tokenId = 1;
        UInt256 vaultDistribution = 10000000;
        UInt256 miningGovernanceDistribution = 3000000;
        const int periodIndex = 2;
        const ulong distributionBlock = 3;
        const ulong nextDistributionBlock = 4;

        // Act
        var distribution = new TokenDistribution(tokenId, vaultDistribution, miningGovernanceDistribution, periodIndex,
                                                 distributionBlock, nextDistributionBlock);

        // Assert
        distribution.Id.Should().Be(0);
        distribution.TokenId.Should().Be(tokenId);
        distribution.VaultDistribution.Should().Be(vaultDistribution);
        distribution.MiningGovernanceDistribution.Should().Be(miningGovernanceDistribution);
        distribution.PeriodIndex.Should().Be(periodIndex);
        distribution.DistributionBlock.Should().Be(distributionBlock);
        distribution.NextDistributionBlock.Should().Be(nextDistributionBlock);
    }

    [Fact]
    public void CreatesExisting_TokenDistribution_Success()
    {
        // Arrange
        const ulong id = 999;
        const ulong tokenId = 1;
        UInt256 vaultDistribution = 10000000;
        UInt256 miningGovernanceDistribution = 3000000;
        const int periodIndex = 2;
        const ulong distributionBlock = 3;
        const ulong nextDistributionBlock = 4;

        // Act
        var distribution = new TokenDistribution(id, tokenId, vaultDistribution, miningGovernanceDistribution, periodIndex,
                                                 distributionBlock, nextDistributionBlock);

        // Assert
        distribution.Id.Should().Be(id);
        distribution.TokenId.Should().Be(tokenId);
        distribution.VaultDistribution.Should().Be(vaultDistribution);
        distribution.MiningGovernanceDistribution.Should().Be(miningGovernanceDistribution);
        distribution.PeriodIndex.Should().Be(periodIndex);
        distribution.DistributionBlock.Should().Be(distributionBlock);
        distribution.NextDistributionBlock.Should().Be(nextDistributionBlock);
    }
}
