namespace Opdex.Platform.WebApi.Models.Responses.Pools
{
    public class ReservesResponseModel
    {
        public string Crs { get; set; }
        public string Src { get; set; }
        public decimal Usd { get; set; }
        public decimal? UsdDailyChange { get; set; }
    }
}
