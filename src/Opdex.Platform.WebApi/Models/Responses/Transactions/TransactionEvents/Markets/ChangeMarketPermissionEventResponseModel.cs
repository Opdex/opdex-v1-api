namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.Markets
{
    public class ChangeMarketPermissionEventResponseModel : TransactionEventResponseModel
    {
        public string Address { get; set;  }
        public string Permission { get; set; }
        public bool IsAuthorized { get; set; }
    }
}
