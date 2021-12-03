using FluentAssertions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.MiningGovernances;
using Opdex.Platform.Domain.Models.Transactions;
using System;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.MiningGovernances;

public class MiningGovernanceContractSummaryTests
{
    [Fact]
    public void CreateNew_MiningGovernanceContractSummary_InvalidBlockHeight_ThrowArgumentOutOfRangeException()
    {
        // Arrange
        // Act
        void Act() => new MiningGovernanceContractSummary(0);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Block height must be greater than zero.");
    }

    [Fact]
    public void Create_MiningGovernanceContractSummary_Success()
    {
        // Arrange
        const ulong blockHeight = 10;

        // Act
        var summary = new MiningGovernanceContractSummary(blockHeight);

        // Assert
        summary.BlockHeight.Should().Be(blockHeight);
    }

    [Fact]
    public void MiningGovernanceContractSummary_InvalidMiningDuration_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        const ulong blockHeight = 10;
        var summary = new MiningGovernanceContractSummary(blockHeight);

        const ulong miningDuration = 0;

        // Act
        void Act() => summary.SetMiningDuration(new SmartContractMethodParameter(miningDuration));

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Mining duration must be greater than zero.");
    }

    [Fact]
    public void MiningGovernanceContractSummary_SetMiningDuration_Success()
    {
        // Arrange
        const ulong blockHeight = 10;
        var summary = new MiningGovernanceContractSummary(blockHeight);

        const ulong miningDuration = 100ul;

        // Act
        summary.SetMiningDuration(new SmartContractMethodParameter(miningDuration));

        // Assert
        summary.MiningDuration.Should().Be(miningDuration);
    }

    [Fact]
    public void MiningGovernanceContractSummary_SetMiningPoolReward_Success()
    {
        // Arrange
        const ulong blockHeight = 10;
        var summary = new MiningGovernanceContractSummary(blockHeight);

        UInt256 reward = 500;

        // Act
        summary.SetMiningPoolReward(new SmartContractMethodParameter(reward));

        // Assert
        summary.MiningPoolReward.Should().Be(reward);
    }

    [Fact]
    public void MiningGovernanceContractSummary_SetMiningPoolsFunded_Success()
    {
        // Arrange
        const ulong blockHeight = 10;
        var summary = new MiningGovernanceContractSummary(blockHeight);

        const uint funded = 4;

        // Act
        summary.SetMiningPoolsFunded(new SmartContractMethodParameter(funded));

        // Assert
        summary.MiningPoolsFunded.Should().Be(funded);
    }

    [Fact]
    public void MiningGovernanceContractSummary_SetNominationPeriodEnd_Success()
    {
        // Arrange
        const ulong blockHeight = 10;
        var summary = new MiningGovernanceContractSummary(blockHeight);

        const ulong periodEnd = 100ul;

        // Act
        summary.SetNominationPeriodEnd(new SmartContractMethodParameter(periodEnd));

        // Assert
        summary.NominationPeriodEnd.Should().Be(periodEnd);
    }

    [Fact]
    public void MiningGovernanceContractSummary_InvalidMinedToken_ThrowsArgumentNullException()
    {
        // Arrange
        const ulong blockHeight = 10;
        var summary = new MiningGovernanceContractSummary(blockHeight);

        Address token = Address.Empty;

        // Act
        void Act() => summary.SetMinedToken(new SmartContractMethodParameter(token));

        // Assert
        // TODO/NOTE: This is the exception thrown in SmartContractMethodParameter when setting a null or empty address
        Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Address value must be set");
    }

    [Fact]
    public void MiningGovernanceContractSummary_SetMinedToken_Success()
    {
        // Arrange
        const ulong blockHeight = 10;
        var summary = new MiningGovernanceContractSummary(blockHeight);

        Address token = "PARwm9EjmivLgqqH1Mroh6zXXNMUiT1GLs";

        // Act
        summary.SetMinedToken(new SmartContractMethodParameter(token));

        // Assert
        summary.MinedToken.Should().Be(token);
    }
}