using System;
using System.Dynamic;
using FluentAssertions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Domain.Models.TransactionLogs.Markets;
using Opdex.Platform.Domain.Models.Transactions;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.Markets;

public class MarketTests
{
    [Fact]
    public void CreateMarket_InvalidAddress_ThrowArgumentNullException()
    {
        // Arrange
        var address = Address.Empty;

        // Act
        void Act() => new Market(address, 5, 10, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", true, true, true, 3, true, 100_000);

        // Assert
        Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Address must be set.");
    }

    [Fact]
    public void CreateMarket_InvalidOwner_ThrowArgumentNullException()
    {
        // Arrange
        var owner = Address.Empty;

        // Act
        void Act() => new Market("PMWrLGcwhr1zboamZQzC5Jk75JyYJSAzoi", 5, 10, owner, true, true, true, 3, true, 100_000);

        // Assert
        Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Owner must be set.");
    }

    [Theory]
    [InlineData(0)]
    public void CreateMarket_InvalidDeployerId_ThrowArgumentOutOfRangeException(ulong deployer)
    {
        // Arrange
        // Act
        void Act() => new Market("PMWrLGcwhr1zboamZQzC5Jk75JyYJSAzoi", deployer, 10, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", true, true, true, 3, true, 100_000);

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
        Address address = "PMWrLGcwhr1zboamZQzC5Jk75JyYJSAzoi";
        var deployerId = 5ul;
        var stakingTokenId = 10ul;
        Address owner = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
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

    [Fact]
    public void SetPendingOwnership_NullValueProvided_ThrowArgumentNullException()
    {
        // Arrange
        var market = new Market(5, "PMWrLGcwhr1zboamZQzC5Jk75JyYJSAzoi", 1, 10, Address.Empty, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", true, true, true, 5, true, 100, 105);
        var pendingOwner = "PTdjXpRFWXrUK7FCHcAjbsPWXaCSefipxh";

        dynamic log = new ExpandoObject();
        log.from = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
        log.to = pendingOwner;

        // Act
        void Act() => market.SetPendingOwnership(null, 510);

        // Assert
        Assert.Throws<ArgumentNullException>(Act);
    }


    [Fact]
    public void SetPendingOwnership_PendingOwner_Updated()
    {
        // Arrange
        var market = new Market(5, "PMWrLGcwhr1zboamZQzC5Jk75JyYJSAzoi", 1, 10, Address.Empty, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", true, true, true, 5, true, 100, 105);
        var pendingOwner = "PTdjXpRFWXrUK7FCHcAjbsPWXaCSefipxh";

        dynamic log = new ExpandoObject();
        log.from = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
        log.to = pendingOwner;

        // Act
        market.SetPendingOwnership(new SetPendingMarketOwnershipLog(log, "PAdS3HnzJ5QhacRuQ5Yb5koAp4XxqswnXi", 5), 100_005);

        // Assert
        market.PendingOwner.Should().Be(pendingOwner);
    }

    [Fact]
    public void SetPendingOwnership_ModifiedBlock_Updated()
    {
        // Arrange
        var market = new Market(5, "PMWrLGcwhr1zboamZQzC5Jk75JyYJSAzoi", 1, 10, Address.Empty, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", true, true, true, 5, true, 100, 105);
        var pendingOwner = "PTdjXpRFWXrUK7FCHcAjbsPWXaCSefipxh";

        dynamic log = new ExpandoObject();
        log.from = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
        log.to = pendingOwner;
        ulong blockHeight = 100_005;

        // Act
        market.SetPendingOwnership(new SetPendingMarketOwnershipLog(log, "PAdS3HnzJ5QhacRuQ5Yb5koAp4XxqswnXi", 5), blockHeight);

        // Assert
        market.ModifiedBlock.Should().Be(blockHeight);
    }

    [Fact]
    public void SetOwnershipClaimed_NullValueProvided_ThrowArgumentNullException()
    {
        // Arrange
        var market = new Market(5, "PMWrLGcwhr1zboamZQzC5Jk75JyYJSAzoi", 1, 10, Address.Empty, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", true, true, true, 5, true, 100, 105);

        // Act
        void Act() => market.SetOwnershipClaimed(null, 99_999);

        // Assert
        Assert.Throws<ArgumentNullException>(Act);
    }

    [Fact]
    public void SetOwnershipClaimed_PreviousModifiedBlock_ThrowArgumentOutOfRangeException()
    {
        // Arrange
        var currentOwner = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
        var pendingOwner = "PR71udY85pAcNcitdDfzQevp6Zar9DizHM";
        var market = new Market(5, "PMWrLGcwhr1zboamZQzC5Jk75JyYJSAzoi", 1, 10, Address.Empty, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", true, true, true, 5, true, 100, 105);

        dynamic log = new ExpandoObject();
        log.from = currentOwner;
        log.to = pendingOwner;

        // Act
        void Act() => market.SetOwnershipClaimed(new ClaimPendingMarketOwnershipLog(log, "PAdS3HnzJ5QhacRuQ5Yb5koAp4XxqswnXi", 5), 104);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act);
    }

    [Fact]
    public void SetOwnershipClaimed_ValidArguments_SetProperties()
    {
        // Arrange
        var currentOwner = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
        var pendingOwner = "PR71udY85pAcNcitdDfzQevp6Zar9DizHM";
        var market = new Market(5, "PMWrLGcwhr1zboamZQzC5Jk75JyYJSAzoi", 1, 10, Address.Empty, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", true, true, true, 5, true, 100, 105);

        dynamic log = new ExpandoObject();
        log.from = currentOwner;
        log.to = pendingOwner;
        ulong blockHeight = 100_010;

        // Act
        market.SetOwnershipClaimed(new ClaimPendingMarketOwnershipLog(log, "PAdS3HnzJ5QhacRuQ5Yb5koAp4XxqswnXi", 5), blockHeight);

        // Assert
        market.PendingOwner.Should().Be(Address.Empty);
        market.Owner.Should().Be(pendingOwner);
        market.ModifiedBlock.Should().Be(blockHeight);
    }

    [Fact]
    public void Update_Values_Updated()
    {
        // Arrange
        var market = new Market(5, "PMWrLGcwhr1zboamZQzC5Jk75JyYJSAzoi", 1, 10, Address.Empty, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", true, true, true, 5, true, 100, 105);

        const ulong block = 99999999;
        Address updatedPendingOwner = "PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh";
        Address updatedOwner = "Pf7LXaWgRKCvMJL7JK7skDpACVauUvuqjB";

        var summary = new MarketContractSummary(block);
        summary.SetPendingOwner(new SmartContractMethodParameter(updatedPendingOwner));
        summary.SetOwner(new SmartContractMethodParameter(updatedOwner));

        // Act
        market.Update(summary);

        // Assert
        market.PendingOwner.Should().Be(updatedPendingOwner);
        market.Owner.Should().Be(updatedOwner);
        market.ModifiedBlock.Should().Be(block);
    }
}