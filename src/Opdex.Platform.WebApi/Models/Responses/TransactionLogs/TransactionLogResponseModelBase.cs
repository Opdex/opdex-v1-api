namespace Opdex.Platform.WebApi.Models.Responses.TransactionLogs
{
    public abstract class TransactionLogResponseModelBase
    {
        public string LogType { get; set; }
        public long TransactionId { get; set; }
        public string Address { get; set; }
        public int SortOrder { get; set; }
    }
}