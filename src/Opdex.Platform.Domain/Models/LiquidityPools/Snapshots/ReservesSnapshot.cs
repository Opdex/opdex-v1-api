using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Opdex.Platform.Domain.Models.LiquidityPools.Snapshots
{
    public class ReservesSnapshot
    {
        public ReservesSnapshot()
        {
            Crs = new Ohlc<ulong>();
            Src = new Ohlc<UInt256>();
            Usd = new Ohlc<decimal>();
        }

        public ReservesSnapshot(IList<ReservesSnapshot> snapshots)
        {
            Crs = new Ohlc<ulong>(snapshots.Select(snapshot => snapshot.Crs).ToList());
            Src = new Ohlc<UInt256>(snapshots.Select(snapshot => snapshot.Src).ToList());
            Usd = new Ohlc<decimal>(snapshots.Select(snapshot => snapshot.Usd).ToList());
        }

        public ReservesSnapshot(Ohlc<ulong> reserveCrs, Ohlc<UInt256> reserveSrc, Ohlc<decimal> reserveUsd)
        {
            Crs = reserveCrs ?? throw new ArgumentNullException(nameof(reserveCrs), "Reserves CRS cannot be null.");
            Src = reserveSrc ?? throw new ArgumentNullException(nameof(reserveSrc), "Reserves SRC cannot be null.");
            Usd = reserveUsd ?? throw new ArgumentNullException(nameof(reserveUsd), "Reserves USD cannot be null.");
        }

        public Ohlc<ulong> Crs { get; }
        public Ohlc<UInt256> Src { get; }
        public Ohlc<decimal> Usd { get; }

        /// <summary>
        /// Update an existing snapshot's values with a new ReservesLog.
        /// </summary>
        /// <param name="log">The new Reserves Log to process</param>
        /// <param name="crsUsd">The CRS token USD price at the time of the transaction.</param>
        internal void Update(ReservesLog log, decimal crsUsd)
        {
            Crs.Update(log.ReserveCrs);
            Src.Update(log.ReserveSrc);
            UpdateUsd(crsUsd, false);
        }

        /// <summary>
        /// Update an existing snapshot by forcing a refresh of the USD totals.
        /// </summary>
        /// <param name="crsUsd">The CRS token USD price to update values with.</param>
        internal void Update(decimal crsUsd)
        {
            UpdateUsd(crsUsd, false);
        }

        /// <summary>
        /// Refresh and reset a reserves snapshot entirely. Rolls over previous closing values and recalculates
        /// new USD values based upon the provided CRS token USD price.
        /// </summary>
        /// <param name="crsUsd">The CRS token USD price to recalculate USD totals.</param>
        internal void Refresh(decimal crsUsd)
        {
            Crs.Refresh(Crs.Close);
            Src.Refresh(Src.Close);
            UpdateUsd(crsUsd, true);
        }

        private void UpdateUsd(decimal crsUsd, bool refresh)
        {
            var totalCrsUsd = MathExtensions.TotalFiat(Crs.Close, crsUsd, TokenConstants.Cirrus.Sats);
            var totalUsd = Math.Round(totalCrsUsd * 2, TokenConstants.Cirrus.Decimals);

            if (refresh) Usd.Refresh(totalUsd);
            else Usd.Update(totalUsd);
        }
    }
}
