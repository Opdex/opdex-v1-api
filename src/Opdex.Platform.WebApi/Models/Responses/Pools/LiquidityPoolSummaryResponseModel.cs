using System;

namespace Opdex.Platform.WebApi.Models.Responses.Pools
{
    public class LiquidityPoolSummaryResponseModel
    {
        public long TransactionCount { get; set; }
        public ReservesResponseModel Reserves { get; set; }
        public RewardsResponseModel Rewards { get; set; }
        public StakingResponseModel Staking { get; set; }
        public VolumeResponseModel Volume { get; set; }
        public CostResponseModel Cost { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}