using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Pools
{
    public class ReservesResponseModel
    {
        public FixedDecimal Crs { get; set; }
        public FixedDecimal Src { get; set; }
        public decimal Usd { get; set; }
        public decimal? UsdDailyChange { get; set; }
    }
}
