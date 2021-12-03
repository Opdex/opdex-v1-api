using FluentAssertions;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Domain.Models.Transactions;
using System;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.Markets;

public class  MarketContractSummaryTests
{
    [Fact]
    public void CreateNew_MarketContractSummary_InvalidBlockHeight_ThrowArgumentOutOfRangeException()
    {
        // Arrange
        // Act
        void Act() => new  MarketContractSummary(0);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Block height must be greater than zero.");
    }

    [Fact]
    public void Create_MarketContractSummary_Success()
    {
        // Arrange
        const ulong blockHeight = 10;

        // Act
        var summary = new  MarketContractSummary(blockHeight);

        // Assert
        summary.BlockHeight.Should().Be(blockHeight);
    }

    [Fact]
    public void MarketContractSummary_SetOwner_Success()
    {
        // Arrange
        const ulong blockHeight = 10;
        var summary = new  MarketContractSummary(blockHeight);

        Address owner = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";

        // Act
        summary.SetOwner(new SmartContractMethodParameter(owner));

        // Assert
        summary.Owner.Should().Be(owner);
    }

    [Fact]
    public void  MarketContractSummary_InvalidOwnerType_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        const ulong blockHeight = 10;
        var summary = new  MarketContractSummary(blockHeight);

        Address owner = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
        const SmartContractParameterType incorrectType = SmartContractParameterType.Byte;

        // Act
        void Act() => summary.SetOwner(new SmartContractMethodParameter(owner.ToString(), incorrectType));

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Owner value must be of Address type.");
    }
}