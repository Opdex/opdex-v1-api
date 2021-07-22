namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.Governances
{
    public class RewardMiningPoolEventResponseModel : TransactionEventResponseModel
    {
        public string StakingPool { get; set; }
        public string MiningPool { get; set; }
        public string Amount { get; set; }
    }
}
