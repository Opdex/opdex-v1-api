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
            Weight = new Ohlc<UInt256>();
            Usd = new Ohlc<decimal>();
        }

        public StakingSnapshot(StakingSnapshot snapshot)
        {
            Weight = snapshot.Weight;
            Usd = snapshot.Usd;
        }

        public StakingSnapshot(Ohlc<UInt256> stakingWeight, Ohlc<decimal> stakingUsd)
        {
            Weight = stakingWeight ?? throw new ArgumentNullException(nameof(stakingWeight), "Staking weight must be provided.");
            Usd = stakingUsd ?? throw new ArgumentNullException(nameof(stakingUsd), "Staking USD must be provided.");;
        }

        public Ohlc<UInt256> Weight { get; }
        public Ohlc<decimal> Usd { get; }

        internal void SetStaking(StakeLog log, decimal stakingTokenUsd)
        {
            UpdateUsd(log.TotalStaked, stakingTokenUsd);
            Weight.Update(log.TotalStaked);
        }

        internal void RefreshStaking(decimal stakingTokenUsd)
        {
            UpdateUsd(Weight.Close, stakingTokenUsd);
        }

        private void UpdateUsd(UInt256 weight, decimal stakingTokenUsd)
        {
            Usd.Update(MathExtensions.TotalFiat(weight, stakingTokenUsd, TokenConstants.Opdex.Sats));
        }
    }
}
