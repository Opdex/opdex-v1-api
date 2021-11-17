using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.OHLC;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;
using System;

namespace Opdex.Platform.Domain.Models.LiquidityPools.Snapshots
{
    public class ReservesSnapshot
    {
        public ReservesSnapshot()
        {
            Crs = 0;
            Src = UInt256.Zero;
            Usd = new OhlcDecimalSnapshot();
        }

        public ReservesSnapshot(ReservesSnapshot snapshots)
        {
            Crs = snapshots.Crs;
            Src = snapshots.Src;
            Usd = snapshots.Usd;
        }

        public ReservesSnapshot(ulong reserveCrs, UInt256 reserveSrc, OhlcDecimalSnapshot reserveUsd)
        {
            Crs = reserveCrs;
            Src = reserveSrc;
            Usd = reserveUsd ?? new OhlcDecimalSnapshot();
        }

        public ulong Crs { get; private set; }
        public UInt256 Src { get; private set; }
        public OhlcDecimalSnapshot Usd { get; }

        internal void SetReserves(ReservesLog log, decimal crsUsd)
        {
            Crs = log.ReserveCrs;
            Src = log.ReserveSrc;
            UpdateUsdReserves(crsUsd);
        }

        internal void RefreshReserves(decimal crsUsd)
        {
            UpdateUsdReserves(crsUsd);
        }

        private void UpdateUsdReserves(decimal crsUsd)
        {
            var totalCrsUsd = MathExtensions.TotalFiat(Crs, crsUsd, TokenConstants.Cirrus.Sats);
            var totalUsd = Math.Round(totalCrsUsd * 2, TokenConstants.Cirrus.Decimals);
            Usd.Update(totalUsd);
        }
    }
}
