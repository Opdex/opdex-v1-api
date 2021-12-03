using FluentAssertions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.MiningPools;
using Opdex.Platform.Domain.Models.TransactionLogs.MiningPools;
using Opdex.Platform.Domain.Models.Transactions;
using System;
using System.Dynamic;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.MiningPools;

public class MiningPoolTests
{
    [Fact]
    public void CreateNewMiningPool_InvalidAddress_ThrowsArgumentNullException()
    {
        // Arrange

        //Act
        void Act() => new MiningPool(1, Address.Empty, 2);

        // Assert
        Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Mining pool address must be set.");
    }

    [Fact]
    public void CreateNewMiningPool_InvalidLiquidityPoolId_ThrowsArgumentOutOfRangeException()
    {
        // Arrange

        //Act
        void Act() => new MiningPool(0, "PARwm9EjmivLgqqH1Mroh6zXXNMUiT1GLs", 2);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Liquidity pool id must be greater than zero.");
    }

    [Fact]
    public void CreateNewMiningPool_Success()
    {
        // Arrange
        const ulong liquidityPoolId = 1;
        Address address = "PARwm9EjmivLgqqH1Mroh6zXXNMUiT1GLs";
        const ulong createdBlock = 2;

        // Act
        var pool = new MiningPool(liquidityPoolId, address, createdBlock);

        // Assert
        pool.LiquidityPoolId.Should().Be(liquidityPoolId);
        pool.Address.Should().Be(address);
        pool.CreatedBlock.Should().Be(createdBlock);
    }

    [Fact]
    public void CrateExistingMiningPool_Success()
    {
        // Arrange
        const ulong id = 1;
        const ulong liquidityPoolId = 2;
        Address address = "PARwm9EjmivLgqqH1Mroh6zXXNMUiT1GLs";
        UInt256 rewardPerBlock = 3;
        UInt256 rewardPerLpt = 4;
        const ulong miningPeriodEndBlock = 100;
        const ulong createdBlock = 5;
        const ulong modifiedBlock = 6;

        // Act
        var pool = new MiningPool(id, liquidityPoolId, address, rewardPerBlock, rewardPerLpt, miningPeriodEndBlock, createdBlock, modifiedBlock);

        // Assert
        pool.Id.Should().Be(id);
        pool.LiquidityPoolId.Should().Be(liquidityPoolId);
        pool.Address.Should().Be(address);
        pool.RewardPerBlock.Should().Be(rewardPerBlock);
        pool.RewardPerLpt.Should().Be(rewardPerLpt);
        pool.MiningPeriodEndBlock.Should().Be(miningPeriodEndBlock);
        pool.CreatedBlock.Should().Be(createdBlock);
        pool.ModifiedBlock.Should().Be(modifiedBlock);
    }

    [Fact]
    public void EnableMiningPool_Success()
    {
        // Arrange
        var miningPool = new MiningPool(1, "PARwm9EjmivLgqqH1Mroh6zXXNMUiT1GLs", 2);

        const ulong modifiedBlock = 500;
        UInt256 rewardPerBlock = 200;
        const ulong miningPeriodEnd = 300;
        UInt256 amount = 100;

        dynamic log = new ExpandoObject();
        log.rewardRate = rewardPerBlock.ToString();
        log.miningPeriodEndBlock = miningPeriodEnd;
        log.amount = amount.ToString();

        // Act
        miningPool.EnableMining(new EnableMiningLog(log, "PARwm9EjmivLgqqH1Mroh6zXXNMUiT1GLs", 0), modifiedBlock);

        // Assert
        miningPool.RewardPerBlock.Should().Be(rewardPerBlock);
        miningPool.MiningPeriodEndBlock.Should().Be(miningPeriodEnd);
        miningPool.ModifiedBlock.Should().Be(modifiedBlock);

    }

    [Fact]
    public void Update_Success()
    {
        // Arrange
        const ulong modifiedBlock = 500;
        const ulong miningPeriodEnd = 1000;
        UInt256 rewardPerLpt = 25;
        UInt256 rewardRate = 2500;
        var miningPool = new MiningPool(1, "PARwm9EjmivLgqqH1Mroh6zXXNMUiT1GLs", 2);

        var summary = new MiningPoolContractSummary(modifiedBlock);
        summary.SetMiningPeriodEnd(new SmartContractMethodParameter(miningPeriodEnd));
        summary.SetRewardPerLpt(rewardPerLpt);
        summary.SetRewardRate(new SmartContractMethodParameter(rewardRate));

        // Act
        miningPool.Update(summary);

        // Assert
        miningPool.RewardPerBlock.Should().Be(rewardRate);
        miningPool.RewardPerLpt.Should().Be(rewardPerLpt);
        miningPool.MiningPeriodEndBlock.Should().Be(miningPeriodEnd);
        miningPool.ModifiedBlock.Should().Be(modifiedBlock);
    }
}