namespace Opdex.Indexer.Domain.Models.LogEvents
{
    public class LogEventBase : ILogEvent
    {
        public string EventType { get; set; }
    }
}