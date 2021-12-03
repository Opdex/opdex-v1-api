using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Transactions;
using System;

namespace Opdex.Platform.Domain.Models.MiningPools;

public class MiningPoolContractSummary
{
    public MiningPoolContractSummary(ulong blockHeight)
    {
        if (blockHeight == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
        }

        BlockHeight = blockHeight;
    }

    public ulong BlockHeight { get; }
    public UInt256? RewardRate { get; private set; }
    public ulong? MiningPeriodEnd { get; private set; }
    public UInt256? RewardPerLpt { get; private set; }

    public void SetRewardRate(SmartContractMethodParameter value)
    {
        if (value.Type != SmartContractParameterType.UInt256)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Reward rate value must be of UInt256 type.");
        }

        RewardRate = value.Parse<UInt256>();
    }

    public void SetMiningPeriodEnd(SmartContractMethodParameter value)
    {
        if (value.Type != SmartContractParameterType.UInt64)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Mining period end value must be of ulong type.");
        }

        MiningPeriodEnd = value.Parse<ulong>();
    }

    public void SetRewardPerLpt(UInt256 reward)
    {
        RewardPerLpt = reward;
    }
}