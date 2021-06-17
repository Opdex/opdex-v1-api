using System;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;

namespace Opdex.Platform.Domain.Models.Pools.Snapshot
{
    public class ReservesSnapshot
    {
        public ReservesSnapshot()
        {
            Crs = "0";
            Src = "0";
            Usd = 0.00m;
        }

        public ReservesSnapshot(string reserveCrs, string reserveSrc, decimal reserveUsd)
        {
            if (!reserveCrs.IsNumeric())
            {
                throw new ArgumentNullException(nameof(reserveCrs), $"{nameof(reserveCrs)} must be a numeric value.");
            }

            if (!reserveSrc.IsNumeric())
            {
                throw new ArgumentNullException(nameof(reserveSrc), $"{nameof(reserveSrc)} must be a numeric value.");
            }

            Crs = reserveCrs;
            Src = reserveSrc;
            Usd = reserveUsd;
        }

        public string Crs { get; private set; }
        public string Src { get; private set; }
        public decimal Usd { get; private set; }

        internal void SetReserves(ReservesLog log, decimal crsUsd, decimal srcUsd, int srcDecimals)
        {
            Crs = log.ReserveCrs.ToString();
            Src = log.ReserveSrc;
            Usd = CalculateReservesUsd(crsUsd, srcUsd, srcDecimals);
        }

        internal void RefreshReserves(decimal crsUsd,  decimal srcUsd, int srcDecimals)
        {
            Usd = CalculateReservesUsd(crsUsd, srcUsd, srcDecimals);
        }

        private decimal CalculateReservesUsd(decimal crsUsd, decimal srcUsd, int srcDecimals)
        {
            var reserveCrsRounded = Crs.ToRoundedDecimal(2, TokenConstants.Cirrus.Decimals);
            var reserveSrcRounded = Src.ToRoundedDecimal(2, srcDecimals);

            var reserveCrsUsd = Math.Round(reserveCrsRounded * crsUsd, 2, MidpointRounding.AwayFromZero);
            var reserveSrcUsd = Math.Round(reserveSrcRounded * srcUsd, 2, MidpointRounding.AwayFromZero);

            return reserveCrsUsd + reserveSrcUsd;
        }
    }
}