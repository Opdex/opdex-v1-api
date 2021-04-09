namespace Opdex.Core.Infrastructure.Abstractions.Data.Models.TransactionLogs
{
    public abstract class LogEntityBase : AuditEntity
    {
        public long Id { get; set; }
        public long TransactionId { get; set; }
        public string Address { get; set; }
        public int SortOrder { get; set; }
    }
}