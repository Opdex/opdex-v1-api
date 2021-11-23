using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents
{
    public class OwnershipEvent : TransactionEvent
    {
        public Address From { get; set; }
        public Address To { get; set; }
    }
}
