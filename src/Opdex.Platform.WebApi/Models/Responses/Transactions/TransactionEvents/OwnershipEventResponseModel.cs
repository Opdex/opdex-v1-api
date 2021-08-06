namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents
{
    public class OwnershipEventResponseModel : TransactionEventResponseModel
    {
        public string From { get; set; }
        public string To { get; set; }
    }
}
