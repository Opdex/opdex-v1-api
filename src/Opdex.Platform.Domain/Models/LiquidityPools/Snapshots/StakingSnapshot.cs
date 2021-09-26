using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Opdex.Platform.Domain.Models.LiquidityPools.Snapshots
{
    public class StakingSnapshot
    {
        public StakingSnapshot()
        {
            Weight = 0;
            Usd = 0.00000000m;
        }

        public StakingSnapshot(IList<StakingSnapshot> snapshots)
        {
            Weight = snapshots.Aggregate(UInt256.Zero, (a,c) => a + c.Weight);
            Usd = snapshots.Aggregate(0.00000000m, (a,c) => a + c.Usd);
        }

        public StakingSnapshot(UInt256 stakingWeight, decimal stakingUsd)
        {
            if (stakingUsd < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(stakingUsd), $"{nameof(stakingUsd)} must be greater or equal to 0.");
            }

            Weight = stakingWeight;
            Usd = stakingUsd;
        }

        public UInt256 Weight { get; private set; }
        public decimal Usd { get; private set; }

        internal void SetStaking(StakeLog log, decimal stakingTokenUsd)
        {
            if (stakingTokenUsd > 0)
            {
                Usd = log.TotalStaked.TotalFiat(stakingTokenUsd, TokenConstants.Opdex.Sats);
            }

            Weight = log.TotalStaked;
        }

        internal void RefreshStaking(decimal stakingTokenUsd)
        {
            Usd = stakingTokenUsd > 0
                ? Weight.TotalFiat(stakingTokenUsd, TokenConstants.Opdex.Sats)
                : 0m;
        }
    }
}
