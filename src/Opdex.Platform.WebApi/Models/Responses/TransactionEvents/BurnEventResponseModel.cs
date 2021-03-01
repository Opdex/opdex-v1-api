namespace Opdex.Platform.WebApi.Models.Responses.TransactionEvents
{
    public class BurnEventResponseModel : TransactionEventResponseModelBase
    {
        public string Sender { get; set; }
        public string To { get; set; }
        public ulong AmountCrs { get; set; }
        public string AmountSrc { get; set; }
    }
}