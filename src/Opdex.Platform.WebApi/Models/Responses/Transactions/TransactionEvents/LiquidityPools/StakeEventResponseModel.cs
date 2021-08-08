namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.LiquidityPools
{
    public abstract class StakeEventResponseModel : TransactionEventResponseModel
    {
        public string Staker { get; set; }
        public string Amount { get; set; }
        public string StakerBalance { get; set; }
        public string TotalStaked { get; set; }
    }
}
