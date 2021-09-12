using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Application.Abstractions.Models.OHLC
{
    public class OhlcBigIntDto
    {
        public UInt256 Open { get; set; }
        public UInt256 High { get; set; }
        public UInt256 Low { get; set; }
        public UInt256 Close { get; set; }
    }
}
