namespace Opdex.Platform.WebApi.Models.Responses.TransactionLogs
{
    public class BurnLogResponseModel : TransactionLogResponseModelBase
    {
        public string Sender { get; set; }
        public string To { get; set; }
        public ulong AmountCrs { get; set; }
        public string AmountSrc { get; set; }
    }
}