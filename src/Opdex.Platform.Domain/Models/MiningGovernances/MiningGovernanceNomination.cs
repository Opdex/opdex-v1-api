using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Blocks;
using System;

namespace Opdex.Platform.Domain.Models.MiningGovernances;

public class MiningGovernanceNomination : BlockAudit
{
    public MiningGovernanceNomination(ulong miningGovernanceId, ulong liquidityPoolId, ulong miningPoolId, bool isNominated, UInt256 weight, ulong createdBlock) : base(createdBlock)
    {
        if (miningGovernanceId < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(miningGovernanceId), "Mining governance id must be greater than 0.");
        }

        if (liquidityPoolId < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(liquidityPoolId), "Liquidity pool id must be greater than 0.");
        }

        if (miningPoolId < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(miningPoolId), "Mining pool id must be greater than 0.");
        }

        MiningGovernanceId = miningGovernanceId;
        LiquidityPoolId = liquidityPoolId;
        MiningPoolId = miningPoolId;
        IsNominated = isNominated;
        Weight = weight;
    }

    public MiningGovernanceNomination(ulong id, ulong miningGovernanceId, ulong liquidityPoolId, ulong miningPoolId, bool isNominated, UInt256 weight, ulong createdBlock, ulong modifiedBlock)
        : base(createdBlock, modifiedBlock)
    {
        Id = id;
        MiningGovernanceId = miningGovernanceId;
        LiquidityPoolId = liquidityPoolId;
        MiningPoolId = miningPoolId;
        IsNominated = isNominated;
        Weight = weight;
    }

    public ulong Id { get; }
    public ulong MiningGovernanceId { get; }
    public ulong LiquidityPoolId { get; }
    public ulong MiningPoolId { get; }
    public bool IsNominated { get; private set; }
    public UInt256 Weight { get; private set; }

    public void SetStatus(bool status, ulong block)
    {
        IsNominated = status;
        SetModifiedBlock(block);
    }

    public void SetWeight(UInt256 weight, ulong block)
    {
        Weight = weight;
        SetModifiedBlock(block);
    }
}