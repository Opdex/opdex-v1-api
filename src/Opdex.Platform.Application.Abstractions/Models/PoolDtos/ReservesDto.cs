using System;

namespace Opdex.Platform.Application.Abstractions.Models.PoolDtos
{
    public class ReservesDto
    {
        public ulong Crs { get; set; }
        public string Src { get; set; }
        public decimal Usd { get; set; }
        public decimal? UsdDailyChange { get; set; }

        public void SetUsdDailyChange(decimal previousUsd)
        {
            if (previousUsd <= 0)
            {
                UsdDailyChange = 0.00m;
                return;
            }

            var usdDailyChange = (Usd - previousUsd) / previousUsd * 100;
            UsdDailyChange = Math.Round(usdDailyChange, 2, MidpointRounding.AwayFromZero);
        }
    }
}
