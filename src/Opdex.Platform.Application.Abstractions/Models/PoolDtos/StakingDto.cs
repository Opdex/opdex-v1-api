namespace Opdex.Platform.Application.Abstractions.Models.PoolDtos
{
    public class StakingDto
    {
        public string Weight { get; set; }
        public decimal Usd { get; set; }
        public decimal? WeightDailyChange { get; set; }
    }
}
