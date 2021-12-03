using FluentAssertions;
using Opdex.Platform.Domain.Models.LiquidityPools.Snapshots;
using System;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.LiquidityPools.Snapshots;

public class RewardsSnapshotTests
{
    [Fact]
    public void CreateRewardsSnapshot_InvalidProviderUsd_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        const decimal providerUsd = -1.00m;

        // Act
        void Act() => new RewardsSnapshot(providerUsd, 1.00m);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain($"{nameof(providerUsd)} must be greater or equal to 0.");
    }

    [Fact]
    public void CreateRewardsSnapshot_InvalidMarketUsd_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        const decimal marketUsd = -1.00m;

        // Act
        void Act() => new RewardsSnapshot(1.25m, marketUsd);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain($"{nameof(marketUsd)} must be greater or equal to 0.");
    }

    [Fact]
    public void CreateRewardsSnapshot_Success()
    {
        // Arrange
        const decimal providerUsd = 1.23m;
        const decimal marketUsd = 1.23m;

        // Act
        var snapshot = new RewardsSnapshot(providerUsd, marketUsd);

        // Assert
        snapshot.ProviderUsd.Should().Be(providerUsd);
        snapshot.MarketUsd.Should().Be(marketUsd);
    }

    [Fact]
    public void CreateRewardsSnapshot_Default_Success()
    {
        // Arrange
        // Act
        var snapshot = new RewardsSnapshot();

        // Assert
        snapshot.ProviderUsd.Should().Be(0.00m);
        snapshot.MarketUsd.Should().Be(0.00m);
    }
}