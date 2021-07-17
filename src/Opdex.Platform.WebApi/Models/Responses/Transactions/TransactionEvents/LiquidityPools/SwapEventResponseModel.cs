namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.LiquidityPools
{
    public class SwapEventResponseModel : TransactionEventResponseModel
    {
        public string Sender { get; set; }
        public string To { get; set; }
        public string AmountCrsIn { get; set; }
        public string AmountSrcIn { get; set; }
        public string AmountCrsOut { get; set; }
        public string AmountSrcOut { get; set; }
        public string SrcToken { get; set; }
    }
}
