using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;
using System;

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

        public ReservesSnapshot(ReservesSnapshot snapshot)
        {
            Crs = new Ohlc<ulong>();
            Src = new Ohlc<UInt256>();
            Usd = new Ohlc<decimal>();

            Crs.Update(snapshot.Crs.Close, true);
            Src.Update(snapshot.Src.Close, true);
            Usd.Update(snapshot.Usd.Close, true);
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

        internal void SetReserves(ReservesLog log, decimal crsUsd)
        {
            Crs.Update(log.ReserveCrs);
            Src.Update(log.ReserveSrc);
            UpdateUsdReserves(crsUsd);
        }

        internal void RefreshReserves(decimal crsUsd)
        {
            UpdateUsdReserves(crsUsd);
        }

        private void UpdateUsdReserves(decimal crsUsd)
        {
            var totalCrsUsd = MathExtensions.TotalFiat(Crs.Close, crsUsd, TokenConstants.Cirrus.Sats);
            var totalUsd = Math.Round(totalCrsUsd * 2, TokenConstants.Cirrus.Decimals);
            Usd.Update(totalUsd);
        }
    }
}
