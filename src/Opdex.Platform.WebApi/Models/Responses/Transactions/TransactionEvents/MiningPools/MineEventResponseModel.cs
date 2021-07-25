namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.MiningPools
{
    public class MineEventResponseModel : TransactionEventResponseModel
    {
        public string Miner { get; set; }
        public string Amount { get; set; }
        public string SubEventType { get; set; }
    }
}
