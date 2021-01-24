namespace Opdex.Core.Domain.Models.TransactionReceipt.LogEvents
{
    public interface ILogEvent
    {
        public string EventType { get; }
    }
}