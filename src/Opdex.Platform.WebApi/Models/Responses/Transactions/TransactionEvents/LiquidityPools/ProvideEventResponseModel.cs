namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.LiquidityPools
{
    public class ProvideEventResponseModel : TransactionEventResponseModel
    {
        public string AmountCrs { get; set; }
        public string AmountSrc { get; set; }
        public string AmountLpt { get; set; }
        public string TokenSrc { get; set; }
        public string TokenLp { get; set; }
        public string SubEventType { get; set; }
    }
}