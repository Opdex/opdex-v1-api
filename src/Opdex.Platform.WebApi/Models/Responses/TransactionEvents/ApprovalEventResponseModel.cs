namespace Opdex.Platform.WebApi.Models.Responses.TransactionEvents
{
    public class ApprovalEventResponseModel : TransactionEventResponseModelBase
    {
        public string Owner { get; set; }
        public string Spender { get; set; }
        public string Amount { get; set; }
    }
}