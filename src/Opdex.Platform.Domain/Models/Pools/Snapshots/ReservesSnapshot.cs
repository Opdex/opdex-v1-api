using System;
using Opdex.Platform.Common;
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

        internal void SetReserves(ReservesLog log, decimal crsUsd, decimal srcUsd, int srcDecimals)
        {
            Crs = log.ReserveCrs;
            Src = log.ReserveSrc;
            Usd = CalculateReservesUsd(crsUsd, srcUsd, srcDecimals);
        }

        internal void RefreshReserves(decimal crsUsd,  decimal srcUsd, int srcDecimals)
        {
            Usd = CalculateReservesUsd(crsUsd, srcUsd, srcDecimals);
        }

        private decimal CalculateReservesUsd(decimal crsUsd, decimal srcUsd, int srcDecimals)
        {
            var reserveCrsRounded = Crs.ToString().ToRoundedDecimal(2, TokenConstants.Cirrus.Decimals);
            var reserveSrcRounded = Src.ToRoundedDecimal(2, srcDecimals);

            var reserveCrsUsd = Math.Round(reserveCrsRounded * crsUsd, 2, MidpointRounding.AwayFromZero);
            var reserveSrcUsd = Math.Round(reserveSrcRounded * srcUsd, 2, MidpointRounding.AwayFromZero);

            return reserveCrsUsd + reserveSrcUsd;
        }
    }
}
