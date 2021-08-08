namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.Tokens
{
    public class ApprovalEventResponseModel : TransactionEventResponseModel
    {
        public string Owner { get; set; }
        public string Spender { get; set; }
        public string Amount { get; set; }
    }
}
