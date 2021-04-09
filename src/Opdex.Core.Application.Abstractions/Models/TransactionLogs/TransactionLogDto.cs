namespace Opdex.Core.Application.Abstractions.Models.TransactionLogs
{
    public abstract class TransactionLogDto
    {
        public long Id { get; set; }
        public string LogType { get; set; }
        public long TransactionId { get; set; }
        public string Address { get; set; }
        public int SortOrder { get; set; }
    }
}