namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents
{
    public class TransactionEventDto
    {
        public long Id { get; set; }
        public string LogType { get; set; }
        public long TransactionId { get; set; }
        public string Contract { get; set; }
        public int SortOrder { get; set; }
    }
}
