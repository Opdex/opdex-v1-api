using Opdex.Platform.Common.Models;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Responses.Pools
{
    public class StakingResponseModel
    {
        public FixedDecimal Weight { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Usd { get; set; }

        public decimal? WeightDailyChange { get; set; }

        public bool? IsActive { get; set; }
        
        public bool? IsNominated { get; set; }
    }
}
