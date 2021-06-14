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
            VolumeCrs = "0";
            VolumeSrc = "0";
            VolumeUsd = 0.00m;
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

            VolumeCrs = volumeCrs;
            VolumeSrc = volumeSrc;
            VolumeUsd = volumeUsd;
        }

        public string VolumeCrs { get; private set; }
        public string VolumeSrc { get; private set; }
        public decimal VolumeUsd { get; private set; }

        internal void SetVolume(SwapLog log, decimal crsUsd)
        {
            var volumeCrs = log.AmountCrsIn + log.AmountCrsOut;
            VolumeCrs = VolumeCrs.Add(volumeCrs.ToString());

            var volumeSrc = log.AmountSrcIn.Add(log.AmountSrcOut);
            VolumeSrc = VolumeSrc.Add(volumeSrc);

            var crsVolumeDecimal = VolumeCrs.ToRoundedDecimal(8, TokenConstants.Cirrus.Decimals);
            VolumeUsd = Math.Round(crsVolumeDecimal * crsUsd, 2, MidpointRounding.AwayFromZero);
        }
    }
}