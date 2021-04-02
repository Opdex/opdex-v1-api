namespace Opdex.Core.Domain.Models.TransactionEvents
{
    public class TransactionEventSummary
    {
        public TransactionEventSummary(long id, long transactionId, long eventId, int eventTypeId, int sortOrder, string contract)
        {
            Id = id;
            TransactionId = transactionId;
            EventId = eventId;
            EventTypeId = eventTypeId;
            SortOrder = sortOrder;
            Contract = contract;
        }
        
        public long Id { get; set; }
        public long TransactionId { get; set; }
        public long EventId { get; set; }
        public int EventTypeId { get; set; }
        public int SortOrder { get; set; }
        public string Contract { get; set; }
    }
}