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
            Weight = new Ohlc<UInt256>();
            Usd = new Ohlc<decimal>();

            Refresh(snapshot.Weight.Close, snapshot.Usd.Close);
        }

        public StakingSnapshot(Ohlc<UInt256> stakingWeight, Ohlc<decimal> stakingUsd)
        {
            Weight = stakingWeight ?? throw new ArgumentNullException(nameof(stakingWeight), "Staking weight must be provided.");
            Usd = stakingUsd ?? throw new ArgumentNullException(nameof(stakingUsd), "Staking USD must be provided.");
        }

        public Ohlc<UInt256> Weight { get; }
        public Ohlc<decimal> Usd { get; }

        /// <summary>
        /// Refreshes the staking snapshot by resetting the OHLC values of properties to equal
        /// the current closing value. Current Close values set all OHLC values for the reset.
        /// </summary>
        internal void Refresh()
        {
            Refresh(Weight.Close, Usd.Close);
        }

        internal void SetStaking(StakeLog log, decimal stakingTokenUsd)
        {
            UpdateUsd(log.TotalStaked, stakingTokenUsd);
            Weight.Update(log.TotalStaked);
        }

        internal void RefreshStaking(decimal stakingTokenUsd)
        {
            UpdateUsd(Weight.Close, stakingTokenUsd);
        }

        private void Refresh(UInt256 weight, decimal usd)
        {
            Weight.Update(weight, true);
            Usd.Update(usd, true);
        }

        private void UpdateUsd(UInt256 weight, decimal stakingTokenUsd)
        {
            Usd.Update(MathExtensions.TotalFiat(weight, stakingTokenUsd, TokenConstants.Opdex.Sats));
        }
    }
}
