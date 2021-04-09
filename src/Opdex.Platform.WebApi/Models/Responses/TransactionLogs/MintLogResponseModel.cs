namespace Opdex.Platform.WebApi.Models.Responses.TransactionLogs
{
    public class MintLogResponseModel : TransactionLogResponseModelBase
    {
        public string Sender { get; set; }
        public ulong AmountCrs { get; set; }
        public string AmountSrc { get; set; }
    }
}