namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.MiningPools
{
    public class CollectMiningRewardsEventResponseModel : TransactionEventResponseModel
    {
        public string Miner { get; set; }
        public string Amount { get; set; }
    }
}
