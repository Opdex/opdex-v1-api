namespace Opdex.Platform.WebApi.Models.Responses.TransactionEvents
{
    public class SwapEventResponseModel : TransactionEventResponseModelBase
    {
        public string Sender { get; set; }
        public string To { get; set; }
        public ulong AmountCrsIn { get; set; }
        public string AmountSrcIn { get; set; }
        public ulong AmountCrsOut { get; set; }
        public string AmountSrcOut { get; set; }
    }
}