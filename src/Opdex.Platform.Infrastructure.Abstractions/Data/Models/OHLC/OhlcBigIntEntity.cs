using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.OHLC
{
    public class OhlcBigIntEntity
    {
        public UInt256 Open { get; set; }
        public UInt256 High { get; set; }
        public UInt256 Low { get; set; }
        public UInt256 Close { get; set; }
    }
}
