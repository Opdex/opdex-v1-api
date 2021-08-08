namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.MiningPools
{
    public class EnableMiningEventResponseModel : TransactionEventResponseModel
    {
        public string Amount { get; set; }
        public string RewardRate { get; set; }
        public ulong MiningPeriodEndBlock { get; set; }
    }
}
