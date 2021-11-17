using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Models.LiquidityPools
{
    public class ReservesDto
    {
        public FixedDecimal Crs { get; set; }
        public FixedDecimal Src { get; set; }
        public decimal Usd { get; set; }
        public decimal DailyUsdChangePercent { get; set; }
    }
}
