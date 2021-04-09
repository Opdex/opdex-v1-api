using System;

namespace Opdex.Core.Infrastructure.Abstractions.Data.Models.TransactionLogs
{
    public class TransactionLogSummaryEntity
    {
        public long Id { get; set; }
        public long TransactionId { get; set; }
        public int LogTypeId { get; set; }
        public long LogId { get; set; }
        public string Contract { get; set; }
        public int SortOrder { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}