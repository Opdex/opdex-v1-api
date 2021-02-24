namespace Opdex.Core.Domain.Models.TransactionReceipt.LogEvents
{
    public interface ILogEvent
    {
        public string EventType { get; }
        public string Address { get; }

        public void SetAddress(string address);

        public void SetTransactionId(long id);
    }
}