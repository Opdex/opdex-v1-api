namespace Opdex.Core.Infrastructure.Abstractions.Data.Models.TransactionEvents
{
    public abstract class EventEntityBase : AuditEntity
    {
        public long Id { get; set; }
        public long TransactionId { get; set; }
        public string Address { get; set; }
    }
}