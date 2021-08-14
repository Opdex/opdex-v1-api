using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.Abstractions.Models.LiquidityPools
{
    public class ReservesDto
    {
        public ulong Crs { get; set; }
        public string Src { get; set; }
        public decimal Usd { get; set; }
        public decimal? UsdDailyChange { get; set; }

        public void SetUsdDailyChange(decimal previousUsd)
        {
            UsdDailyChange = Usd.PercentChange(previousUsd);
        }
    }
}