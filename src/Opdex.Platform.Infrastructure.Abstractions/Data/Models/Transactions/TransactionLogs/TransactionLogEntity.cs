using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.Transactions.TransactionLogs
{
    public class TransactionLogEntity : AuditEntity
    {
        public long Id { get; set; }
        public long TransactionId { get; set; }
        public int LogTypeId { get; set; }
        public string Contract { get; set; }
        public int SortOrder { get; set; }
        public string Details { get; set; }
    }
}