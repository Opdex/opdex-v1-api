namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.Tokens
{
    public class TransferEventResponseModel : TransactionEventResponseModel
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Amount { get; set; }
    }
}
