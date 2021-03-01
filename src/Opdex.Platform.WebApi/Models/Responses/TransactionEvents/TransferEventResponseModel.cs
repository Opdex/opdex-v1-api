namespace Opdex.Platform.WebApi.Models.Responses.TransactionEvents
{
    public class TransferEventResponseModel : TransactionEventResponseModelBase
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Amount { get; set; }
    }
}