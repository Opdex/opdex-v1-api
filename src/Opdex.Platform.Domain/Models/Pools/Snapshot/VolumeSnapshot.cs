using System;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;

namespace Opdex.Platform.Domain.Models.Pools.Snapshot
{
    public class VolumeSnapshot
    {
        public VolumeSnapshot()
        {
            Crs = "0";
            Src = "0";
            Usd = 0.00m;
        }

        public VolumeSnapshot(string volumeCrs, string volumeSrc, decimal volumeUsd)
        {
            if (!volumeCrs.IsNumeric())
            {
                throw new ArgumentNullException(nameof(volumeCrs), $"{nameof(volumeCrs)} must be a numeric value.");
            }

            if (!volumeSrc.IsNumeric())
            {
                throw new ArgumentNullException(nameof(volumeSrc), $"{nameof(volumeSrc)} must be a numeric value.");
            }

            Crs = volumeCrs;
            Src = volumeSrc;
            Usd = volumeUsd;
        }

        public string Crs { get; private set; }
        public string Src { get; private set; }
        public decimal Usd { get; private set; }

        internal void SetVolume(SwapLog log, decimal crsUsd)
        {
            var volumeCrs = log.AmountCrsIn + log.AmountCrsOut;
            Crs = Crs.Add(volumeCrs.ToString());

            var volumeSrc = log.AmountSrcIn.Add(log.AmountSrcOut);
            Src = Src.Add(volumeSrc);

            // Todo: Incorrect, accurate volume would track USD price vs tokenInAmount which includes fees and slippage.
            var crsVolumeDecimal = Crs.ToRoundedDecimal(8, TokenConstants.Cirrus.Decimals);
            Usd = Math.Round(crsVolumeDecimal * crsUsd, 2, MidpointRounding.AwayFromZero);
        }
    }
}