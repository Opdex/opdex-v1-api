namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.LiquidityPools
{
    public class StakeEventResponseModel : TransactionEventResponseModel
    {
        public string Staker { get; set; }
        public string Amount { get; set; }
        public string SubEventType { get; set; }
    }
}
