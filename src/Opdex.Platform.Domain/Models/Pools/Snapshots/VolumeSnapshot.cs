using System;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;

namespace Opdex.Platform.Domain.Models.Pools.Snapshots
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
                throw new ArgumentOutOfRangeException(nameof(volumeCrs), $"{nameof(volumeCrs)} must be a numeric value.");
            }

            if (!volumeSrc.IsNumeric())
            {
                throw new ArgumentOutOfRangeException(nameof(volumeSrc), $"{nameof(volumeSrc)} must be a numeric value.");
            }

            if (volumeUsd < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(volumeUsd), $"{nameof(volumeUsd)} must be greater or equal to 0.");
            }

            Crs = volumeCrs;
            Src = volumeSrc;
            Usd = volumeUsd;
        }

        public string Crs { get; private set; }
        public string Src { get; private set; }
        public decimal Usd { get; private set; }

        internal void SetVolume(SwapLog log, decimal crsUsd, decimal srcUsd, int srcDecimals)
        {
            var volumeCrs = log.AmountCrsIn + log.AmountCrsOut;
            Crs = Crs.Add(volumeCrs.ToString());

            var volumeSrc = log.AmountSrcIn.Add(log.AmountSrcOut);
            Src = Src.Add(volumeSrc);

            var crsVolumeDecimal = log.AmountCrsIn.ToString().ToRoundedDecimal(8, TokenConstants.Cirrus.Decimals);
            var srcVolumeDecimal = log.AmountSrcIn.ToRoundedDecimal(8, srcDecimals);
            var crsUsdVolume = crsVolumeDecimal * crsUsd;
            var srcUsdVolume = srcVolumeDecimal * srcUsd;

            Usd += Math.Round(crsUsdVolume + srcUsdVolume, 2, MidpointRounding.AwayFromZero);
        }
    }
}