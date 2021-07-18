namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents
{
    public abstract class TransactionEventDto
    {
        public long Id { get; set; }
        public long TransactionId { get; set; }
        public TransactionEventType EventType { get; set; }
        public string Contract { get; set; }
        public int SortOrder { get; set; }
    }
}
