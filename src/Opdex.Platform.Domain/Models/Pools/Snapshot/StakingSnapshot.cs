using System;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;

namespace Opdex.Platform.Domain.Models.Pools.Snapshot
{
    public class StakingSnapshot
    {
        public StakingSnapshot()
        {
            StakingWeight = "0";
            StakingUsd = 0.00m;
        }

        public StakingSnapshot(string stakingWeight, decimal stakingUsd)
        {
            if (!stakingWeight.IsNumeric())
            {
                throw new ArgumentNullException(nameof(stakingWeight), $"{nameof(stakingWeight)} must be a numeric value.");
            }

            StakingWeight = stakingWeight;
            StakingUsd = stakingUsd;
        }

        public string StakingWeight { get; private set; }
        public decimal StakingUsd { get; private set; }

        internal void SetStaking(StakeLog log, decimal stakingTokenUsd)
        {
            if (stakingTokenUsd > 0)
            {
                StakingUsd = CalculateStakingUsd(log.TotalStaked, stakingTokenUsd);
            }

            StakingWeight = log.TotalStaked;
        }

        internal void RefreshStaking(decimal stakingTokenUsd)
        {
            StakingUsd = stakingTokenUsd > 0
                ? CalculateStakingUsd(StakingWeight, stakingTokenUsd)
                : 0m;
        }

        private decimal CalculateStakingUsd(string totalStaked, decimal stakingTokenUsd)
        {
            const int precision = 2;
            var odxDecimal = totalStaked.ToRoundedDecimal(precision, TokenConstants.Opdex.Decimals);
            var odxWeightUsd = Math.Round(odxDecimal * stakingTokenUsd, precision, MidpointRounding.AwayFromZero);

            return odxWeightUsd;
        }
    }
}