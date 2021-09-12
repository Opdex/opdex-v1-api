using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.Transactions.TransactionLogs
{
    public class TransactionLogEntity
    {
        public long Id { get; set; }
        public long TransactionId { get; set; }
        public int LogTypeId { get; set; }
        public Address Contract { get; set; }
        public int SortOrder { get; set; }
        public string Details { get; set; }
    }
}
