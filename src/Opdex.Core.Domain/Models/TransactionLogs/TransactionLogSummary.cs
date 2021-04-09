namespace Opdex.Core.Domain.Models.TransactionLogs
{
    public class TransactionLogSummary
    {
        public TransactionLogSummary(long id, long transactionId, long logId, int logTypeId, int sortOrder, string contract)
        {
            Id = id;
            TransactionId = transactionId;
            LogId = logId;
            LogTypeId = logTypeId;
            SortOrder = sortOrder;
            Contract = contract;
        }
        
        public long Id { get; set; }
        public long TransactionId { get; set; }
        public long LogId { get; set; }
        public int LogTypeId { get; set; }
        public int SortOrder { get; set; }
        public string Contract { get; set; }
    }
}