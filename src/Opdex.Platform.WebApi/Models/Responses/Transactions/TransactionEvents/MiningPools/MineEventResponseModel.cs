namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.MiningPools
{
    public abstract class MineEventResponseModel : TransactionEventResponseModel
    {
        public string Miner { get; set; }
        public string Amount { get; set; }
        public string TotalSupply { get; set; }
        public string MinerBalance { get; set; }
    }
}
