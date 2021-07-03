using System;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;

namespace Opdex.Platform.Domain.Models.Pools.Snapshots
{
    public class ReservesSnapshot
    {
        public ReservesSnapshot()
        {
            Crs = 0;
            Src = "0";
            Usd = 0.00m;
        }

        public ReservesSnapshot(ulong reserveCrs, string reserveSrc, decimal reserveUsd)
        {
            if (!reserveSrc.IsNumeric())
            {
                throw new ArgumentOutOfRangeException(nameof(reserveSrc), $"{nameof(reserveSrc)} must be a numeric value.");
            }

            if (reserveUsd < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(reserveUsd), $"{nameof(reserveUsd)} must be greater or equal to 0.");
            }

            Crs = reserveCrs;
            Src = reserveSrc;
            Usd = reserveUsd;
        }

        public ulong Crs { get; private set; }
        public string Src { get; private set; }
        public decimal Usd { get; private set; }

        internal void SetReserves(ReservesLog log, decimal crsUsd, decimal srcUsd, ulong srcSats)
        {
            Crs = log.ReserveCrs;
            Src = log.ReserveSrc;
            Usd = CalculateReservesUsd(crsUsd, srcUsd, srcSats);
        }

        internal void RefreshReserves(decimal crsUsd,  decimal srcUsd, ulong srcSats)
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
