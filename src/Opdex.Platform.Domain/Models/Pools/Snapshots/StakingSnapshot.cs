using System;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;

namespace Opdex.Platform.Domain.Models.Pools.Snapshots
{
    public class StakingSnapshot
    {
        public StakingSnapshot()
        {
            Weight = "0";
            Usd = 0.00m;
        }

        public StakingSnapshot(string stakingWeight, decimal stakingUsd)
        {
            if (!stakingWeight.IsNumeric())
            {
                throw new ArgumentOutOfRangeException(nameof(stakingWeight), $"{nameof(stakingWeight)} must be a numeric value.");
            }

            if (stakingUsd < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(stakingUsd), $"{nameof(stakingUsd)} must be greater or equal to 0.");
            }

            Weight = stakingWeight;
            Usd = stakingUsd;
        }

        public string Weight { get; private set; }
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
