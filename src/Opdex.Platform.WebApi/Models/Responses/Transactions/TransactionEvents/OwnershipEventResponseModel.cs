using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents
{
    public class OwnershipEventResponseModel : TransactionEventResponseModel
    {
        public Address From { get; set; }
        public Address To { get; set; }
    }
}
