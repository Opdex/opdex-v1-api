namespace Opdex.Platform.WebApi.Models.Responses.Pools
{
    public class StakingResponseModel
    {
        public string Weight { get; set; }
        public decimal Usd { get; set; }
        public decimal? WeightDailyChange { get; set; }
        public bool? IsActive { get; set; }
    }
}
