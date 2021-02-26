namespace Opdex.Core.Application.Abstractions.Models.TransactionEvents
{
    public abstract class TransactionEventDto
    {
        public long Id { get; set; }
        public string EventType { get; set; }
        public long TransactionId { get; set; }
        public string Address { get; set; }
        public int SortOrder { get; set; }
    }
}