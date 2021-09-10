using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Application.Abstractions.Models.LiquidityPools
{
    public class VolumeDto
    {
        public ulong Crs { get; set; }
        public UInt256 Src { get; set; }
        public decimal Usd { get; set; }
    }
}
