namespace Opdex.Core.Infrastructure.Abstractions.Data.Models.TransactionEvents
{
    public class EventEntityBase
    {
        public long Id { get; set; }
        public long TransactionId { get; set; }
    }
}