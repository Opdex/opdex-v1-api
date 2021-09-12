using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.Markets
{
    public class ChangeMarketPermissionEventResponseModel : TransactionEventResponseModel
    {
        public Address Address { get; set; }
        public string Permission { get; set; }
        public bool IsAuthorized { get; set; }
    }
}
