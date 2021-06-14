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
            ReserveCrs = "0";
            ReserveSrc = "0";
            ReserveUsd = 0.00m;
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

            ReserveCrs = reserveCrs;
            ReserveSrc = reserveSrc;
            ReserveUsd = reserveUsd;
        }

        public string ReserveCrs { get; private set; }
        public string ReserveSrc { get; private set; }
        public decimal ReserveUsd { get; private set; }

        internal void SetReserves(ReservesLog log, decimal crsUsd)
        {
            ReserveCrs = log.ReserveCrs.ToString();
            ReserveSrc = log.ReserveSrc;
            ReserveUsd = CalculateReservesUsd(crsUsd);
        }

        internal void RefreshReserves(decimal crsUsd)
        {
            ReserveUsd = CalculateReservesUsd(crsUsd);
        }

        private decimal CalculateReservesUsd(decimal crsUsd)
        {
            var reserveCrsRounded = ReserveCrs.ToRoundedDecimal(2, TokenConstants.Cirrus.Decimals);

            // * 2, for reserve Crs USD amount and reserve Src, they are equal
            return Math.Round(reserveCrsRounded * crsUsd * 2, 2, MidpointRounding.AwayFromZero);
        }
    }
}