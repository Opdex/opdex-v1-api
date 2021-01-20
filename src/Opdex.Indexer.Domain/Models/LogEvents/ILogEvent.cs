namespace Opdex.Indexer.Domain.Models
{
    public interface ILogEvent
    {
        public string EventType { get; set; }
    }
}