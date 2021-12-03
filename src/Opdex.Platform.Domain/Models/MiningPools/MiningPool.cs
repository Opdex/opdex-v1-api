using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Blocks;
using Opdex.Platform.Domain.Models.TransactionLogs.MiningPools;
using System;

namespace Opdex.Platform.Domain.Models.MiningPools;

public class MiningPool : BlockAudit
{
    public MiningPool(ulong liquidityPoolId, Address address, ulong createdBlock) : base(createdBlock)
    {
        if (address == Address.Empty)
        {
            throw new ArgumentNullException(nameof(address), "Mining pool address must be set.");
        }

        if (liquidityPoolId < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(liquidityPoolId), "Liquidity pool id must be greater than zero.");
        }

        Address = address;
        LiquidityPoolId = liquidityPoolId;
        RewardPerBlock = UInt256.Zero;
        RewardPerLpt = UInt256.Zero;
        MiningPeriodEndBlock = 0;
    }

    public MiningPool(ulong id, ulong liquidityPoolId, Address address, UInt256 rewardPerBlock, UInt256 rewardPerLpt, ulong miningPeriodEndBlock,
                      ulong createdBlock, ulong modifiedBlock) : base(createdBlock, modifiedBlock)
    {
        Id = id;
        LiquidityPoolId = liquidityPoolId;
        Address = address;
        RewardPerBlock = rewardPerBlock;
        RewardPerLpt = rewardPerLpt;
        MiningPeriodEndBlock = miningPeriodEndBlock;
    }

    public ulong Id { get; }
    public ulong LiquidityPoolId { get; }
    public Address Address { get; }
    public UInt256 RewardPerBlock { get; private set; }
    public UInt256 RewardPerLpt { get; private set; }
    public ulong MiningPeriodEndBlock { get; private set; }

    public void EnableMining(EnableMiningLog log, ulong block)
    {
        RewardPerBlock = log.RewardRate;
        MiningPeriodEndBlock = log.MiningPeriodEndBlock;
        SetModifiedBlock(block);
    }

    public void Update(MiningPoolContractSummary summary)
    {
        if (summary.RewardRate.HasValue) RewardPerBlock = summary.RewardRate.Value;
        if (summary.RewardPerLpt.HasValue) RewardPerLpt = summary.RewardPerLpt.Value;
        if (summary.MiningPeriodEnd.HasValue) MiningPeriodEndBlock = summary.MiningPeriodEnd.Value;
        SetModifiedBlock(summary.BlockHeight);
    }
}