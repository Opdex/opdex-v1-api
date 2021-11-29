using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Models.LiquidityPools
{
    public class CostDto
    {
        public FixedDecimal CrsPerSrc { get; set; }
        public FixedDecimal SrcPerCrs { get; set; }
    }
}
