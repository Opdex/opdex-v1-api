using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using System;

namespace Opdex.Platform.Domain.Models.MiningGovernances;

public class MiningGovernanceContractNominationSummary
{
    public MiningGovernanceContractNominationSummary(Address stakingPool, UInt256 weight)
    {
        if (stakingPool == Address.Empty)
        {
            throw new ArgumentNullException(nameof(stakingPool), "Liquidity pool address must be provided.");
        }

        LiquidityPool = stakingPool;
        StakingWeight = weight;
    }

    public Address LiquidityPool { get; }
    public UInt256 StakingWeight { get; }
}