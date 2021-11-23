using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.Tokens
{
    public class TransferEvent : TransactionEvent
    {
        public Address From { get; set; }
        public Address To { get; set; }
        public FixedDecimal Amount { get; set; }
    }
}
