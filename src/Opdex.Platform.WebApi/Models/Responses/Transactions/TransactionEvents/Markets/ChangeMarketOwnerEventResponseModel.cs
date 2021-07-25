namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.Markets
{
    public class ChangeMarketOwnerEventResponseModel : TransactionEventResponseModel
    {
        public string From { get; set; }
        public string To { get; set; }
    }
}
