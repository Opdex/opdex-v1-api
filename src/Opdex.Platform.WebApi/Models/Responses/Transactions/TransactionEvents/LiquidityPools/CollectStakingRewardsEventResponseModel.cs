namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.LiquidityPools
{
    public class CollectStakingRewardsEventResponseModel : TransactionEventResponseModel
    {
        public string Staker { get; set; }
        public string Reward { get; set; }
    }
}
