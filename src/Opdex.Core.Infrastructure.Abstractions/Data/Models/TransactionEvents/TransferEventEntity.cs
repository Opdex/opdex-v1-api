namespace Opdex.Core.Infrastructure.Abstractions.Data.Models.TransactionEvents
{
    public class TransferEventEntity : EventEntityBase
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Amount { get; set; }
    }
}