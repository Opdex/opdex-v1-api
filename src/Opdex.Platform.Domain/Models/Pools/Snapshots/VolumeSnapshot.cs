using System;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;

namespace Opdex.Platform.Domain.Models.Pools.Snapshots
{
    public class VolumeSnapshot
    {
        public VolumeSnapshot()
        {
            Crs = 0;
            Src = "0";
            Usd = 0.00m;
        }

        public VolumeSnapshot(ulong volumeCrs, string volumeSrc, decimal volumeUsd)
        {
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

        public ulong Crs { get; private set; }
        public string Src { get; private set; }
        public decimal Usd { get; private set; }

        internal void SetVolume(SwapLog log, decimal crsUsd, decimal srcUsd, ulong srcSats)
        {
            Crs += log.AmountCrsIn + log.AmountCrsOut;

            var volumeSrc = log.AmountSrcIn.Add(log.AmountSrcOut);
            Src = Src.Add(volumeSrc);

            var crsVolume = log.AmountCrsIn.TotalFiat(crsUsd, TokenConstants.Cirrus.Sats);
            var srcVolume = log.AmountSrcIn.TotalFiat(srcUsd, srcSats);

            Usd += Math.Round(crsVolume + srcVolume, 2, MidpointRounding.AwayFromZero);
        }
    }
}
