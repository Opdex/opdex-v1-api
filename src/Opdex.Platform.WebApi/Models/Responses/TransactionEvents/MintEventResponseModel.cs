namespace Opdex.Platform.WebApi.Models.Responses.TransactionEvents
{
    public class MintEventResponseModel : TransactionEventResponseModelBase
    {
        public string Sender { get; set; }
        public ulong AmountCrs { get; set; }
        public string AmountSrc { get; set; }
    }
}