using Opdex.Platform.Common.Enums;

namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents
{
    public abstract class TransactionEventResponseModel
    {
        public TransactionEventType EventType { get; set; }
        public string Contract { get; set; }
        public int SortOrder { get; set; }
    }
}
