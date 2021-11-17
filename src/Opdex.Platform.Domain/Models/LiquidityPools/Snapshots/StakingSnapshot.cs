using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;
using System;

namespace Opdex.Platform.Domain.Models.LiquidityPools.Snapshots
{
    public class StakingSnapshot
    {
        public StakingSnapshot()
        {
            Weight = 0;
            Usd = 0.00000000m;
        }

        public StakingSnapshot(StakingSnapshot snapshot)
        {
            Weight = snapshot.Weight;
            Usd = snapshot.Usd;
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
                Usd = MathExtensions.TotalFiat(log.TotalStaked, stakingTokenUsd, TokenConstants.Opdex.Sats);
            }

            Weight = log.TotalStaked;
        }

        internal void RefreshStaking(decimal stakingTokenUsd)
        {
            Usd = stakingTokenUsd > 0
                ? MathExtensions.TotalFiat(Weight, stakingTokenUsd, TokenConstants.Opdex.Sats)
                : 0m;
        }
    }
}
