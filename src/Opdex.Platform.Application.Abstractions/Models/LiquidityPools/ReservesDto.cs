using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Application.Abstractions.Models.LiquidityPools
{
    public class ReservesDto
    {
        public ulong Crs { get; set; }
        public UInt256 Src { get; set; }
        public decimal Usd { get; set; }
        public decimal? UsdDailyChange { get; set; }

        public void SetUsdDailyChange(decimal previousUsd)
        {
            UsdDailyChange = Usd.PercentChange(previousUsd);
        }
    }
}
