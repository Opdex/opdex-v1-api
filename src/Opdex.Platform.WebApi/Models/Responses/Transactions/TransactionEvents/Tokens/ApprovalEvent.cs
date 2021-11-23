using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.Tokens
{
    public class ApprovalEvent : TransactionEvent
    {
        public Address Owner { get; set; }
        public Address Spender { get; set; }
        public FixedDecimal Amount { get; set; }
    }
}
