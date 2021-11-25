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
            Weight = new Ohlc<UInt256>();
            Usd = new Ohlc<decimal>();
        }

        public StakingSnapshot(IList<StakingSnapshot> snapshots)
        {
            Weight = new Ohlc<UInt256>(snapshots.Select(snapshot => snapshot.Weight).ToList());
            Usd = new Ohlc<decimal>(snapshots.Select(snapshot => snapshot.Usd).ToList());
        }

        public StakingSnapshot(Ohlc<UInt256> stakingWeight, Ohlc<decimal> stakingUsd)
        {
            Weight = stakingWeight ?? throw new ArgumentNullException(nameof(stakingWeight), "Staking weight must be provided.");
            Usd = stakingUsd ?? throw new ArgumentNullException(nameof(stakingUsd), "Staking USD must be provided.");
        }

        public Ohlc<UInt256> Weight { get; }
        public Ohlc<decimal> Usd { get; }

        /// <summary>
        /// Update an existing snapshot's values with a new StakeLog.
        /// </summary>
        /// <param name="log">The new Stake Log to process</param>
        /// <param name="stakingTokenUsd">The staking token USD price at the time of the transaction.</param>
        internal void Update(StakeLog log, decimal stakingTokenUsd)
        {
            Weight.Update(log.TotalStaked);

            if (Weight.Close > 0 && stakingTokenUsd == 0)
            {
                throw new ArgumentNullException("Update issue");
            }

            UpdateUsd(log.TotalStaked, stakingTokenUsd, false);
        }

        /// <summary>
        /// Update an existing snapshot by forcing a refresh of the USD totals.
        /// </summary>
        /// <param name="stakingTokenUsd">The staking token USD price to update values with.</param>
        internal void Update(decimal stakingTokenUsd)
        {
            UpdateUsd(Weight.Close, stakingTokenUsd, false);
        }

        /// <summary>
        /// Refresh and reset a staking snapshot entirely. Rolls over previous closing staking weight and recalculates
        /// new USD staking values based upon the provided staking token USD price.
        /// </summary>
        /// <param name="stakingTokenUsd">The staking token USD price to recalculate USD totals.</param>
        internal void Refresh(decimal stakingTokenUsd)
        {
            Weight.Refresh(Weight.Close);

            if (Weight.Close > 0 && stakingTokenUsd == 0)
            {
                throw new ArgumentNullException("Refresh issue");
            }


            UpdateUsd(Weight.Close, stakingTokenUsd, true);
        }

        private void UpdateUsd(UInt256 weight, decimal stakingTokenUsd, bool refresh)
        {
            if (weight > 0 && stakingTokenUsd == 0)
            {
                throw new ArgumentNullException("Getting Somewhere");
            }

            var usd = MathExtensions.TotalFiat(weight, stakingTokenUsd, TokenConstants.Opdex.Sats);

            if (refresh) Usd.Refresh(usd);
            else Usd.Update(usd);
        }
    }
}
