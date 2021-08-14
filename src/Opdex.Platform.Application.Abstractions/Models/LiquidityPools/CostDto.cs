using Opdex.Platform.Application.Abstractions.Models.OHLC;

namespace Opdex.Platform.Application.Abstractions.Models.LiquidityPools
{
    public class CostDto
    {
        public OhlcBigIntDto CrsPerSrc { get; set; }
        public OhlcBigIntDto SrcPerCrs { get; set; }
    }
}
