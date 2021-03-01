namespace Opdex.Platform.WebApi.Models.Responses.TransactionEvents
{
    public abstract class TransactionEventResponseModelBase
    {
        public string EventType { get; set; }
        public long TransactionId { get; set; }
        public string Address { get; set; }
        public int SortOrder { get; set; }
    }
}