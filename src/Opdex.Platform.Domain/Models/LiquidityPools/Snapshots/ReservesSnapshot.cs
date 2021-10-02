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
            Crs = 0;
            Src = UInt256.Zero;
            Usd = 0.00000000m;
        }

        public ReservesSnapshot(ReservesSnapshot snapshots)
        {
            Crs = snapshots.Crs;
            Src = snapshots.Src;
            Usd = snapshots.Usd;
        }

        public ReservesSnapshot(ulong reserveCrs, UInt256 reserveSrc, decimal reserveUsd)
        {
            if (reserveUsd < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(reserveUsd), $"{nameof(reserveUsd)} must be greater or equal to 0.");
            }

            Crs = reserveCrs;
            Src = reserveSrc;
            Usd = reserveUsd;
        }

        public ulong Crs { get; private set; }
        public UInt256 Src { get; private set; }
        public decimal Usd { get; private set; }

        internal void SetReserves(ReservesLog log, decimal crsUsd, decimal srcUsd, ulong srcSats)
        {
            Crs = log.ReserveCrs;
            Src = log.ReserveSrc;
            Usd = CalculateReservesUsd(crsUsd, srcUsd, srcSats);
        }

        internal void RefreshReserves(decimal crsUsd, decimal srcUsd, ulong srcSats)
        {
            Usd = CalculateReservesUsd(crsUsd, srcUsd, srcSats);
        }

        private decimal CalculateReservesUsd(decimal crsUsd, decimal srcUsd, ulong srcSats)
        {
            var totalCrsUsd = Crs.TotalFiat(crsUsd, TokenConstants.Cirrus.Sats);
            var totalSrcUsd = Src.TotalFiat(srcUsd, srcSats);

            return totalCrsUsd + totalSrcUsd;
        }
    }
}
