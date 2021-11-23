using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Responses.Pools
{
    public abstract class LiquidityPoolSummaryResponseModel
    {
        [Range(0, double.MaxValue)]
        public long TransactionCount { get; set; }

        public ReservesResponseModel Reserves { get; set; }

        public RewardsResponseModel Rewards { get; set; }

        public StakingResponseModel Staking { get; set; }

        public VolumeResponseModel Volume { get; set; }
        
        public CostResponseModel Cost { get; set; }
    }
}