using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Pools
{
    public class StakingResponseModel
    {
        public FixedDecimal Weight { get; set; }
        public decimal Usd { get; set; }
        public decimal? WeightDailyChange { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsNominated { get; set; }
    }
}
